# Contribute guidelines

We are always looking for people to help improve Impostor-Staging.

## Code

- If you are implementing or fixing an issue, please comment on the issue first so work is not duplicated.
- If you find an issue that's related to security (yes, availability is part of this), please report privately using the info stated in SECURITY.md.
- If you want to implementing a new feature, create an issue first describing the issue so we know about it. We are always open for discussion or questions on [Discord](https://discord.gg/Mk3w6Tb).
- Don't commit unnecessary changes to the codebase or debugging code.
- Write meaningful commit messages in English. Please squash fixup commits.
- Please try to follow the code style of the rest of the codebase. An `.editorconfig` file has been provided to keep consistency.
- Stylecop is also enabled: please make sure your PR compiles without warnings.

## Pull requests

- We expect that you fully understand the code you are committing. We strongly discourage submitting code by someone or something else that you don't understand. This includes skidding or its 21st century equivalent, vibe coding.
- Only make pull requests to the `dev` branch. Backports to old versions can only be done after the original PR is merged. Bringing your PR to the main branch is done as part of the release train.
- Only implement one feature or bugfix per pull request to keep it easy to understand.
- Expect comments or questions on your pull request from the project maintainers. We try to keep the code as consistent and maintainable as possible.
- Each pull request should come from a new branch in your fork, it should have a meaningful name.
- Include a test plan in your pull request: How should we test your code? For an API change, please add some sample code in Impostor.Api.Example that showcases the feature, for an issue affecting a mod, include that mod in your test plan.
- After opening your PR, check if the build passes: PR's that fail CI will not be reviewed.
- We try to respond to pull requests within a week. If you think we might have missed it, let us know on [Discord](https://discord.gg/Mk3w6Tb).

## Release Trains

To make sure that Impostor's `stable` branch is well, stable, we run release trains every few weeks to merge a bunch of PR's, run a test on a few production servers, then merge the PR's to the `stable` branch.

- PR's that want to join a release train should generally not depend on each other or conflict with each other.
- PR's have to be approved to be part of a release train.
- If a flaw is found in a PR that's part of a release train, 2 things can happen: the flaw is accepted and is fixed in a later release OR the flaw is big enough that the entire PR is reverted.

If you have any questions, let us know.
