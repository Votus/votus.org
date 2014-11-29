Feature: Event Store Management
	As a System Administrator
	I want to be able to manage the Event Store
	So that I can resolve data issues at runtime

Background: 
	Given I navigate to the Admin Dashboard

Scenario: Republish all events
	Given an event exists in the Event Store
	When I republish all events
	Then the event is republished
