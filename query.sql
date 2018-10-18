-- We need to grab the lowest DiagnosisID for each Member's Category
with MostSevereDiagnosis (MemberID, SevereDiagnosisID) as (
    select 
        md.MemberID
        ,min(md.DiagnosisID) 
    from
        MemberDiagnosis md
    group by
        md.MemberID
),

-- We want to get the lowest CategoryID for each member and Category Score is not a factor
MostSevereCategory (MemberID, SevereCategoryID) as (
    select
        md.MemberID
        , min(dc.DiagnosisCategoryID)
    from
        MemberDiagnosis md
    left join DiagnosisCategoryMap dcm
        on md.DiagnosisID = dcm.DiagnosisID
    left join DiagnosisCategory dc
        on dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
    group by
        md.MemberID
)

select 
    m.MemberID as 'Member ID'
    ,m.FirstName as 'First Name'
    ,m.LastName as 'Last Name'
    ,msd.SevereDiagnosisID as 'Most Severe Diagnosis ID'
    ,d.DiagnosisDescription as 'Most Severe Diagnosis Description'
    ,msc.SevereCategoryID as 'Category ID'
    ,dc.CategoryDescription as 'Category Description'
    ,dc.CategoryScore as 'Category Score'
    -- We need to null coalesce Severe CategoryIDs that do not exist with 1
    ,coalesce(msc.SevereCategoryID, 1) as 'Is Most Severe Category'
from Member m
left join MostSevereDiagnosis msd
    on m.MemberID = msd.MemberID
left join Diagnosis d
    on msd.SevereDiagnosisID = d.DiagnosisID
left join MostSevereCategory msc
    on m.MemberID = msc.MemberID
left join DiagnosisCategory dc
    on msc.SevereCategoryID = dc.DiagnosisCategoryID