# Benchmarking

- Came up with 3 ways to tackle the problem
  - Fetch the emails from db and find the difference inside the program
  - Insert the provided emails into a temporary table and find the difference joining those tables
  - Select the emails that are both provided in the csv and db. Then in the program find the difference of the emails provided in csv and this result


- Benchmarks
  - The benchmarks were done on a machine with i5-10500H CPU and 16GB of RAM, Using C# 9.0 and MySQL 8.0.30 
  - `n` - number of emails from CSV
  - `m` - number of emails from DB

| n=10 m=1000000   | Fetching     | InsertingWithSQL  | IntersectionWithEF | 
|------------------|--------------|-------------------|--------------------|           
| indexed          | 1.15 sec     | 0.001 sec         | 0.003 sec          |        
| unindexed        | 0.96 sec     | 0.8785 sec        | 0.575 sec          |

| n=1000 m=1000000 | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 1.22 sec     | 0.043 sec         | 0.049 sec          |  
| unindexed        | 0.96 sec     | 0.978 sec         | 0.781 sec          |  

| n=10000 m=10000  | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 0.01 sec     | 0.082 sec         | 0.073 sec          |  
| unindexed        | 0.01 sec     | 0.087 sec         | 0.070 sec          |  
  
| n=1000 m=10000   | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 0.0090 sec   | 0.006 sec         | 0.006 sec          |  
| unindexed        | 0.0089 sec   | 0.013 sec         | 0.010 sec          |  
  

| n=100 m=1000000  | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 1.08 sec     | 0.019 sec         | 0.018 sec          |  
| unindexed        | 1.01 sec     | 0.884 sec         | 0.765 sec          |  


| n=100 m=1000     | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 0.0010 sec   | 0.00058 sec       | 0.00099 sec        |  
| unindexed        | 0.0019 sec   | 0.00099 sec       | 0.00099 sec        |  


| n=100 m=10       | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 0.00099 sec  | 0.000098 sec      | 0.00099 sec        |  
| unindexed        | 0.00010 sec  | 0.00098 sec       | 0.001 sec          |  


| n=100 m=10000    | Fetching     | InsertingWithSQL  | IntersectionWithEF |  
|------------------|--------------|-------------------|--------------------|  
| indexed          | 0.0089 sec   | 0.0010 sec        | 0.0019 sec         |  
| unindexed        | 0.0080 sec   | 0.0099 sec        | 0.0091 sec         |  

- First approach (`Fetching`)
  - This approach is useful when the database rows are of about the same number as the provided list
and this number is large(not 10 for example). 
Even with the table using indexes this option runs faster. 
Using indexes doesn't really help, in contrast, as the benchmarks show, in general it slows get operation.

- Second approach (`InsertingWithSQL`)
  - This approach has a lot of things to do. It needs to insert all provided emails into a 
temporary table and then join it with the original table to get the results.
 This is why it performs good when `m >> n` or `n` is a small number.

- Third approach (`IntersectionWithEF`)
  - This approach in a sense combines the best of the both worlds. Some work is propagated to 
MySQL Server (find the intersection), some is done inside the program.
In general, this approach performs better than `InsertingWithSQL` on unindexed tables.

# Including Redis

In 2nd and 3rd approaches there is not really much we can cache. Meanwhile, in 1st approach
when we get all users, they can be stored in in-memory cache of redis. Using redis 
has a disadvantage however, if w e get all the users, put them in cache with 
an example TTL (time to live) of  1 min, in the subsequent requests in 1 min we
won't get the recently added use rs (added in the last minute). 
As they say - you don't get some thing for nothing. Let's benchmark the 1st approach
with and without Redis. 
 
| n=10 m=1000000   | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) | 
|------------------|------------ |------------------------------|-------------------------------|        
| indexed          | 1.03 sec    | 1.15 sec                     | 0.0035 sec                    |      
| unindexed        | 1.01 sec    | 1.23 sec                     | 0.0033 sec                    | 

| n=1000 m=1000000 | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) | 
|------------------|------------ |------------------------------|-------------------------------|
| indexed          | 1.11 sec    | 1.32 sec                     | 0.001 sec                     |
| unindexed        | 1.001 sec   | 1.28 sec                     | 0.001 sec                     |

| n=10000 m=10000  | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) | 
|------------------|------------ |------------------------------|-------------------------------|
| indexed          | 0.01 sec    | 0.015 sec                    | 0.0020 sec                    |
| unindexed        | 0.01 sec    | 0.016 sec                    | 0.0019 sec                    |
 
| n=1000 m=10000   | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) | 
|------------------|------------ |------------------------------|-------------------------------|
| indexed          | 0.0089 sec  | 0.059 sec                    | 0.0009 sec                    |
| unindexed        | 0.0090 sec  | 0.012 sec                    | 0.0010 sec                    |


| n=100 m=1000000  | Fetching    | Fetching_cached (first time) |  Fetching_cached (second time)|
|------------------|------------ |------------------------------|-------------------------------|
| indexed          | 1.12 sec    | 1.31 sec                     | 0.001 sec                     |
| unindexed        | 0.98 sec    | 1.21 sec                     | 0.001 sec                     |


| n=100 m=1000     | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) |    
|------------------|------------ |------------------------------|-------------------------------|   
| indexed          | 0.0019 sec  | 0.018 sec                    | 0.0009 sec                    |    
| unindexed        | 0.0010 sec  | 0.056 sec                    | 0.0010 sec                    |     


| n=100 m=10       | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) |   
|------------------|------------ |------------------------------|-------------------------------|    
| indexed          | 0.00099 sec | 0.0020 sec                   | 0.0009 sec                    |       
| unindexed        | 0.00010 sec | 0.0019 sec                   | 0.0009 sec                    |      


| n=100 m=10000    | Fetching    | Fetching_cached (first time) | Fetching_cached (second time) |       
|------------------|------------ |------------------------------|-------------------------------|    
| indexed          | 0.0090 sec  | 0.016 sec                    | 0.0005 sec                    |
| unindexed        | 0.0085 sec  | 0.062 sec                    | 0.0015 sec                    |

As expected, cached version runs  a bit slow first time, when it needs to access the db, but once the value is present
in the cache, the difference is paramount. Also, here it does not make a difference whether the table is indexed
or not. To sum up, Redis can increase the performance, but will it increase or are the compromises acceptable depends
on the situation (Is it ok to not receive most recent data? How often are new users added to db? How long should the 
TTL be for cached values?)
