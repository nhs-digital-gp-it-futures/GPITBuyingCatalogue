IF NOT EXISTS(SELECT 1 FROM catalogue.AssociatedServices WHERE PracticeReorganisationType IS NOT NULL)
BEGIN
  update [catalogue].AssociatedServices SET PracticeReorganisationType = 0 -- Default everything to none

  update [catalogue].AssociatedServices SET PracticeReorganisationType = 3 -- 1 Merger and 2 Split
      FROM [catalogue].AssociatedServices s
        left join [catalogue].[CatalogueItems] ci on s.CatalogueItemId = ci.id
      where ci.name = 'Practice Reorganisation'

  update [catalogue].AssociatedServices SET PracticeReorganisationType = 1 -- Merger 
      FROM [catalogue].AssociatedServices s
        left join [catalogue].[CatalogueItems] ci on s.CatalogueItemId = ci.id
      where ci.name = 'Practice Merge'

  update [catalogue].AssociatedServices SET PracticeReorganisationType = 2 -- Split 
      FROM [catalogue].AssociatedServices s
        left join [catalogue].[CatalogueItems] ci on s.CatalogueItemId = ci.id
      where ci.name = 'Practice Split'
END
