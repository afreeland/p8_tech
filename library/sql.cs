using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace afreeland.export
{
    public class Diagnosis
    {
        private IConfigurationSection _config;
        public Diagnosis(){
              // Lets load our config file
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            _config = configuration.GetSection("pulseDB");
        }

        private void execSQL(string query) {
            try
            {
                // Build connection string
                System.Data.SqlClient.SqlConnectionStringBuilder connBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
                connBuilder.DataSource = _config["host"];   
                connBuilder.UserID = _config["username"];              
                connBuilder.Password = _config["password"];      
                connBuilder.InitialCatalog = _config["db"];

                // Connect to SQL
                using (SqlConnection connection = new SqlConnection(connBuilder.ConnectionString))
                {
                    // Open up a new connection
                    connection.Open();

                    
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;


                    reader = cmd.ExecuteReader();
                    string jsonOutput = string.Empty;
                    if (reader.HasRows)
                    {
                        
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        
                        jsonOutput = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                        Console.WriteLine(jsonOutput);
                    }else{
                        Console.WriteLine("This member does not have any data");
                    }
                    
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        public void Fetch(int memberID){
            Console.WriteLine($"Fetching data for member {memberID.ToString()}");

            var sql = $@"
                -- We need to grab the lowest DiagnosisID for each Member's Category
                with MostSevereDiagnosis (MemberID, SevereDiagnosisID, diagCategoryID) as (
                    select 
                        md.MemberID
                        ,min(md.DiagnosisID) as 'SevereDiagnosisID'
                        , dc.DiagnosisCategoryID
                    from
                        MemberDiagnosis md
                    left join DiagnosisCategoryMap dcm
                        on md.DiagnosisID = dcm.DiagnosisID
                    left join DiagnosisCategory dc
                        on dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
                    group by
                        md.MemberID
                        , dc.DiagnosisCategoryID
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
                        md.MemberID, dc.DiagnosisCategoryID
                )

                select distinct
                    m.MemberID as 'Member ID'
                    ,m.FirstName as 'First Name'
                    ,m.LastName as 'Last Name'
                    ,msd.SevereDiagnosisID as 'Most Severe Diagnosis ID'
                    ,d.DiagnosisDescription as 'Most Severe Diagnosis Description'
                    ,msc.SevereCategoryID as 'Category ID'
                    ,dc.CategoryDescription as 'Category Description'
                    ,dc.CategoryScore as 'Category Score'
                    -- We need verify if our current rows category id is indeed the lowest for 
                    , case when msc.SevereCategoryID = (select min(SevereCategoryID) from MostSevereCategory dmap where MemberID = m.MemberID) 
                            then 
                                1 
                            else 
                                case when msc.SevereCategoryID is null then 1 else 0 end
                        end as 'Is Most Severe Category'
                        
                        
                from Member m
                left join MostSevereDiagnosis msd
                    on m.MemberID = msd.MemberID
                left join Diagnosis d
                    on msd.SevereDiagnosisID = d.DiagnosisID
                left join MostSevereCategory msc
                    on msd.diagCategoryID = msc.SevereCategoryID
                left join DiagnosisCategory dc
                    on msc.SevereCategoryID = dc.DiagnosisCategoryID

                where m.MemberID = {memberID}
            ";
            
            execSQL(sql);
        }
        
    }
}
