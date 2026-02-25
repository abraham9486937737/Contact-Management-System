# Run Application from GitHub Repository URL (Best Approach)

A GitHub repository page itself (`github.com/...`) cannot run an ASP.NET Core server app.

Best approach: connect this repo to **Render** for automatic deploys from GitHub and get a live URL like:

`https://contact-management-system.onrender.com`

## One-time setup

1. Go to Render: https://render.com
2. Sign in with GitHub.
3. Click **New +** → **Blueprint**.
4. Select this repository:
   - `abraham9486937737/Contact-Management-System`
5. Render will detect `render.yaml` in this repo.
6. Click **Apply**.

Render will build and deploy automatically.

## Your live URL

After deploy finishes, open the service URL shown in Render dashboard.

This is the URL you can share with anyone (family/friends) and it will work from browser.

## Auto deploy on every push

Any `git push` to `main` triggers a new deployment automatically.

## Notes

- App uses SQLite with persistent disk at `/var/data/ContactManagement.db`.
- First deploy may take a few minutes.
- Free plan can sleep when idle; first request may take extra time.
