---
description: Create a new Git branch from an existing branch
argument-hint: <source-branch> <new-branch>
---

Create a new Git branch.

Steps:
1. Verify the working tree is clean. If not, stop and explain why.
2. Verify that the source branch exists.
3. Checkout the source branch.
4. Pull the latest changes from origin.
5. Create a new branch from the source branch.
6. Checkout the new branch.
7. Display the current branch name.
8. Confirm the branch was created successfully.

Arguments:
- Source Branch: $1
- New Branch: $2

Do not commit or push anything.
