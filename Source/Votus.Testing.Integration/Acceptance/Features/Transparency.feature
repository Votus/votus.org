Feature: Transparency
	As a Voter
	I want visibility in to the inner workings of the system
	So that I can have a higher level of confidence that it works as advertised

Scenario: Votus system version number is visible
	Given the Voter navigates to the Homepage
	Then the version number is visible

Scenario: Votus environment name is visible
	Given the Voter navigates to the Homepage
	Then the environment name is visible
