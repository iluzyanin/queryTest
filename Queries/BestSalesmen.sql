SELECT 
    SM.Id,
    SM.FirstName,
    SM.LastName        
FROM Salesmen SM
JOIN Sales S ON SM.Id = S.SalesmanId
GROUP BY SM.Id, SM.FirstName, SM.LastName
HAVING SUM(S.AMOUNT) >= @amount