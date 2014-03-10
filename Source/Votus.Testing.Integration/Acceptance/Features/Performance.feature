Feature: Performance
	As a Voter
	I want the website to perform well
	So that I don't have to wait for it

Scenario: New Ideas are quickly visible on the Homepage
	When a Voter submits a new Idea
	Then the Idea appears within 5 seconds