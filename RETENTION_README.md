# Retention Period Design Document

## Requirements


- Take the number to keep as a parameter
- Output the releases that should be kept
- Log why a release is kept
- Focus on the algorithm rather than a user interface

** Note: Release.json and Deployment.json had bad data

## Rationale

The number of releases are growing, this will fill up the space eventually, hence
we are introducing a configuration parameter n to store n recently deployment releases
per environment per product

## Design

Brute Force Strategy:
Create a list of all the releases associated for the n recent per environment per product.
Subtract that list from the list of all releases, the resulting list of releases are the one that should be deleted

Better approach:
We know that the releases and deployments are immutable, hence we can calculate the resulting n releases
by creating a new table that tracks deployment, release, product and creation time. If there exists more than n
records for a given product and environment pair, then delete the oldest record.
Followed by this, delete all the releases that are not present in the above mentioned table.


## Implementation

- Implemented Basic Web Application using .net core and react
- Implemented Dockerfile
- Implemented Unit Tests
