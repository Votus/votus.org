Feature: Goals
	As a Voter
	I want to add goals to an idea 
	So that I can describe to other Voters why the idea is good

Scenario: Voter can add a Goal to an Idea
	Given at least 1 idea exists
	When a Voter navigates to the Homepage
	And a Voter submits a valid Goal to the Idea
	Then the Goal appears under the Idea

Scenario: Voter cannot add invalid Goal to an Idea
	Given at least 1 idea exists
	When a Voter navigates to the Homepage
	And a Voter submits an invalid Goal to the Idea
	Then the error "Please say a few words about your goal" is displayed