# Deploy to Hugging Face (Free Public URL)

Yes — you can host this project for free on **Hugging Face Spaces** using Docker.

## 1) Create a new Space

1. Open: https://huggingface.co/new-space
2. Owner: your account
3. Space name: `contact-management-system` (or any name)
4. SDK: **Docker**
5. Visibility: **Public**
6. Create Space

## 2) Connect your GitHub repository

In the new Space page:

1. Go to **Settings** → **Repository**
2. Choose **Import from GitHub**
3. Select repository: `abraham9486937737/Contact-Management-System`
4. Branch: `main`

Hugging Face will auto-build using the repository `Dockerfile`.

## 3) Environment variables (recommended)

In Space **Settings** → **Variables and secrets**, add:

- `DISABLE_HTTPS_REDIRECTION=true`
- `ASPNETCORE_ENVIRONMENT=Production`
- `SQLITE_DB_PATH=/data/ContactManagement.db`

## 4) Persistent storage (important)

If available in your Space plan, enable persistent storage and mount `/data`.

This keeps your SQLite data after restarts.

## 5) Your public URL

After build is successful, your app URL will be:

`https://<your-space-name>.hf.space`

Example:

`https://contact-management-system.hf.space`

## Notes

- First startup can be slow while Space wakes up.
- Free tier may sleep when idle.
- Any push to GitHub `main` can trigger rebuild depending on your Space sync settings.
