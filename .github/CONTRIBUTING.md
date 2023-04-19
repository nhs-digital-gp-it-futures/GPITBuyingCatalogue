# Contributing Guidelines

Welcome to the GP IT Buying Catalogue project. The following document will help in your understanding of how to contribute changes to this repository.

## Pull Requests
___

When raising a Pull Request, these criteria should be followed.

* **Changes must be feature complete (See Draft Pull Requests section)**. Pushing a branch and raising a Pull Request are two mutually exclusive events. Pull Requests, whether they're Draft or not, should only be raised when your changes are feature complete.
* **Changes should be isolated**. Don't aim to change the world and every feature in a single Pull Request.
* **Changes should be accompanied by test coverage (See Draft Pull Requests section)**. Tests should be written prior to raising a Pull Request.
* **Don't hog the build agent**. Pull requests shouldn't be used in order to run tests. All tests **must** be run locally prior to raising a Pull Request.
* **Don't rebase once a review has begun**. Rebasing is a very powerful tool that can also introduce problems. No rebasing should occur if a Pull Request review has begun whereby a developer or tester has added review comments.
* **Use a meaningful Pull Request title**. The title will become the commit message when the Pull Request is squash merged and the commit messages in `develop` are used in the release notes. As such, titles must clearly outline what the feature or change is.

### **Draft Pull Requests**
___

It isn't always the case that you'll have feature complete work to publish for review. For instance, if you're developing a proof of concept then it's better to get feedback earlier so you can continue down the correct path. In such a scenario, it's okay to adapt the aforementioned guidelines marked above. The guidelines marked above must be met prior to marking a Draft Pull Request as Ready for Review.

However, Draft Pull Requests should not be used for every Pull Request that you raise and Pull Requests should not be raised as soon as the first commit is pushed.  

One should only be raised in either of the following scenarios:

1. It's a proof of concept that you'd like to demonstrate to the team.  
or
2. All of the aforementioned criteria have been met.

## Commits
___

Please ensure that commit messages are concise and clearly explain the change. Commit messages are used to understand the features that have made it into a release and are subsequently shared in a release note. They shouldn't mystify the change itself.

[See here for more](https://cbea.ms/git-commit/).

Optional, but commits on a branch should be squashed, via a rebase, prior to officially raising a Pull Request in order to preserve a clean commit history. This makes it easier to track review changes on a commit by commit basis.