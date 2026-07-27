---
description: Review, commit, and push all project changes to the current branch
argument-hint: [optional context/hint]
---

Review all modified files before committing and pushing.

Steps:
1. Run git status.
2. Review the diff.
3. Ensure there are no obvious mistakes, debug code, commented code, or secrets.
4. Stage all required files.
5. Analyze the staged diff and write a meaningful commit message yourself, describing what changed and why. If context is given below, use it to guide the message:

$ARGUMENTS

6. Commit the staged changes.
7. Determine the current branch name.
8. Push the commit to `origin` on the current branch (set the upstream with `-u` if it isn't already tracking a remote branch).
9. Show:
   - Commit hash
   - Commit message
   - Files included in the commit
   - Branch and remote pushed to

If the push fails (e.g. branch protection rejects it, or history has diverged), stop and explain the error clearly — do not force-push.
