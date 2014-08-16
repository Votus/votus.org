Feature: Ideas
	As a Voter
	I want to submit community improvement ideas to gain support for my concerns
	so that they are more likely to be resolved

Scenario: Voter submits a new Idea
	When a Voter submits a new Idea
	Then the idea appears in the Ideas list

Scenario: Voter submits a new Idea with a tag
	When a Voter submits a new idea with a tag
	Then the idea appears in the Ideas list

Scenario: Test ideas are hidden by default
	Given a test Idea exists in the Ideas List
	When a Voter navigates to the Homepage (no exclude tag)
	Then ideas created for testing purposes do not appear

Scenario: Test ideas can be shown
	Given a test Idea exists in the Ideas List
	When a Voter navigates to the Homepage (no exclude tag)
	And the Voter removes test data filter
	Then ideas created for testing purposes appear

Scenario: Voter can view all ideas
	Given an Idea exists in the Ideas List
	When a Voter navigates to the Homepage
	And the Voter removes test data filter
	Then the Voter can view all ideas

Scenario: Error displayed when title is not provided
	When a Voter navigates to the Homepage
	And a Voter submits a new idea with an invalid title
	Then the error "It would be cool if you could say a few words about your idea!" is displayed

Scenario: API returns error when new idea title is invalid
	When a Voter submits a new idea with title "" via API
	Then the error "It would be cool if you could say a few words about your idea!" is returned