# Find list difference with db table

Each approach has a corresponding function in `UserFilter`.
The provided email list is assumed to be in csv file.  
`n` - number of emails in csv file.
`m` - number of emails in db.

- `Fetching` this method takes advantage of C# `HashSet` for finding the difference easily. Sets use hash functions internally and are a good match for the problem.
The algorithm is like this: all the emails are fetched from db, wrapped in a set data structure, as are the provided emails, then their difference is calculated.

- `InsertingWithSQL` this method delegates all the work to the MySQL server. The provided emails are inserted into a temporary table, performs a join to find the needed emails.
This approach is worse the first one when the `n` and `m` numbers are equivalent or `n > m`. It may be better when `n << m`, but here comes the third method.

- `IntersectionWithEF` this method fetches the emails which are both in db and in the provided list. 
Thus optimizing performance, as there is no need to get all emails from db or insert anything. Then the difference of the list and 
this intersection is calculated again using C# `HashSet`.

- `docker-compose up` for running MySQL and Redis in docker.
- `Run All Tests` in the `Test` toolbar is for running all benchmarks provided, You can look for the results output in the `FunctionsExecutionTime` file that is directed in he `Output` folder.
