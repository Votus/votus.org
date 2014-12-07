Feature: Ideas
	As a Voter
	I want to submit community improvement ideas to gain support for my concerns
	so that they are more likely to be resolved

Scenario: Voter can submit an Idea
	When a Voter submits an Idea
	Then the Idea appears in the Ideas List

Scenario: Voter can submit an Idea with a Tag
	When a Voter submits an Idea with a Tag
	Then the Idea appears in the Ideas List

Scenario: Test Ideas are hidden by default
	Given a test Idea exists in the Ideas List
	When a Voter navigates to the Homepage (no exclude tag)
	Then Ideas created for testing purposes do not appear

Scenario: Test Ideas can be shown
	Given a test Idea exists in the Ideas List
	When a Voter navigates to the Homepage (no exclude tag)
	And the Voter removes test data filter
	Then Ideas created for testing purposes appear

Scenario: Voter can view all Ideas
	Given an Idea exists in the Ideas List
	When a Voter navigates to the Homepage
	And the Voter removes test data filter
	Then the Voter can view all Ideas

Scenario: Error displayed when Title is not provided
	When a Voter navigates to the Homepage
	And a Voter submits an Idea with an invalid Title
	Then the error "It would be cool if you could say a few words about your idea!" is displayed

Scenario: API returns error when new idea Title is invalid
	When a Voter submits an Idea with Title "" via API
	Then the error "It would be cool if you could say a few words about your idea!" is returned