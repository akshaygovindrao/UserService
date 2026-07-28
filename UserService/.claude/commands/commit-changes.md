---
description: Review and commit all project changes
argument-hint: [optional context/hint]
---

Review all modified files before committing.

Steps:
1. Run git status.
2. Review the diff.
3. Ensure there are no obvious mistakes, debug code, commented code, or secrets.
4. Stage all required files.
5. Analyze the staged diff and write a meaningful commit message yourself, describing what changed and why. If context is given below, use it to guide the message:

$ARGUMENTS

6. Show:
   - Commit hash
   - Commit message
   - Files included in the commit

Do not push to the remote repository.
