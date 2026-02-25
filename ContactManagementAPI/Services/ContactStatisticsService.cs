using ContactManagementAPI.Models;
using ContactManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Services
{
    public class ContactStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public ContactStatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContactStatistics> GetStatisticsAsync()
        {
            var totalContacts = await _context.Contacts.CountAsync();
            var contactsWithEmail = await _context.Contacts.Where(c => !string.IsNullOrEmpty(c.Email)).CountAsync();
            var contactsWithPhone = await _context.Contacts.Where(c => 
                !string.IsNullOrEmpty(c.Mobile1) || !string.IsNullOrEmpty(c.Mobile2) || !string.IsNullOrEmpty(c.Mobile3)).CountAsync();
            
            var topCities = await _context.Contacts
                .Where(c => !string.IsNullOrEmpty(c.City))
                .GroupBy(c => c.City)
                .Select(g => new CityStatistic { City = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var contactsByGroup = await _context.Contacts
                .Include(c => c.Group)
                .GroupBy(c => c.Group == null ? "Unassigned" : c.Group.Name)
                .Select(g => new GroupStatistic { GroupName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return new ContactStatistics
            {
                TotalContacts = totalContacts,
                ContactsWithEmail = contactsWithEmail,
                ContactsWithPhone = contactsWithPhone,
                TopCities = topCities,
                ContactsByGroup = contactsByGroup,
                LastUpdated = DateTime.Now
            };
        }

        // Detect potential duplicates
        public async Task<List<ContactDuplicate>> FindDuplicatesAsync()
        {
            var duplicates = new List<ContactDuplicate>();
            var contacts = await _context.Contacts.ToListAsync();

            for (int i = 0; i < contacts.Count; i++)
            {
                for (int j = i + 1; j < contacts.Count; j++)
                {
                    var similarity = CalculateSimilarity(contacts[i], contacts[j]);
                    if (similarity >= 0.7) // 70% similarity threshold
                    {
                        duplicates.Add(new ContactDuplicate
                        {
                            Contact1Id = contacts[i].Id,
                            Contact1Name = $"{contacts[i].FirstName} {contacts[i].LastName}",
                            Contact2Id = contacts[j].Id,
                            Contact2Name = $"{contacts[j].FirstName} {contacts[j].LastName}",
                            SimilarityScore = similarity
                        });
                    }
                }
            }

            return duplicates.OrderByDescending(x => x.SimilarityScore).ToList();
        }

        private double CalculateSimilarity(Contact c1, Contact c2)
        {
            double score = 0;
            int factors = 0;

            // Compare names
            if (!string.IsNullOrEmpty(c1.FirstName) && !string.IsNullOrEmpty(c2.FirstName))
            {
                if (c1.FirstName.Equals(c2.FirstName, StringComparison.OrdinalIgnoreCase))
                    score += 1;
                else if (LevenshteinDistance(c1.FirstName, c2.FirstName) <= 2)
                    score += 0.8;
                factors++;
            }

            if (!string.IsNullOrEmpty(c1.LastName) && !string.IsNullOrEmpty(c2.LastName))
            {
                if (c1.LastName.Equals(c2.LastName, StringComparison.OrdinalIgnoreCase))
                    score += 1;
                else if (LevenshteinDistance(c1.LastName, c2.LastName) <= 2)
                    score += 0.8;
                factors++;
            }

            // Compare emails
            if (!string.IsNullOrEmpty(c1.Email) && !string.IsNullOrEmpty(c2.Email))
            {
                if (c1.Email.Equals(c2.Email, StringComparison.OrdinalIgnoreCase))
                    score += 2; // High weight for email match
                factors++;
            }

            // Compare phones
            if (!string.IsNullOrEmpty(c1.Mobile1) && !string.IsNullOrEmpty(c2.Mobile1))
            {
                if (c1.Mobile1.Equals(c2.Mobile1))
                    score += 2; // High weight for phone match
                factors++;
            }

            return factors > 0 ? score / (factors * 1.5) : 0;
        }

        private int LevenshteinDistance(string s1, string s2)
        {
            var length1 = s1.Length;
            var length2 = s2.Length;
            var d = new int[length1 + 1, length2 + 1];

            for (int i = 0; i <= length1; i++)
                d[i, 0] = i;

            for (int j = 0; j <= length2; j++)
                d[0, j] = j;

            for (int i = 1; i <= length1; i++)
                for (int j = 1; j <= length2; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }

            return d[length1, length2];
        }
    }

    public class ContactStatistics
    {
        public int TotalContacts { get; set; }
        public int ContactsWithEmail { get; set; }
        public int ContactsWithPhone { get; set; }
        public List<CityStatistic> TopCities { get; set; } = new();
        public List<GroupStatistic> ContactsByGroup { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class CityStatistic
    {
        public string City { get; set; }
        public int Count { get; set; }
    }

    public class GroupStatistic
    {
        public string GroupName { get; set; }
        public int Count { get; set; }
    }

    public class ContactDuplicate
    {
        public int Contact1Id { get; set; }
        public string Contact1Name { get; set; }
        public int Contact2Id { get; set; }
        public string Contact2Name { get; set; }
        public double SimilarityScore { get; set; }
    }
}
