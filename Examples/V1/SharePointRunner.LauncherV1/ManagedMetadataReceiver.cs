using CsvHelper.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;

namespace SharePointRunner.LauncherV1
{
    /// <summary>
    /// Enumeration of taxonomy element types
    /// </summary>
    public enum Type
    {
        TermStore,
        TermGroup,
        TermSet,
        Term
    }

    /// <summary>
    /// Taxonomy element informations
    /// </summary>
    public class TermInfo
    {
        public Type Type { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class TermInfoMap : ClassMap<TermInfo>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TermInfoMap()
        {
            Map(m => m.Type);
            Map(m => m.Name);
        }
    }

    public class ManagedMetadataReceiver : Receiver
    {
        private CsvWriterWrapper<TermInfo, TermInfoMap> managedMetadataFileWriter;

        public override void OnTenantRunningStart(Tenant tenant)
        {
            tenant.Context.Load(tenant,
                t => t.RootSiteUrl);
            tenant.Context.ExecuteQuery();

            string managedMetadataFileName = $"AuditManagedMaetadata--{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            managedMetadataFileWriter = new CsvWriterWrapper<TermInfo, TermInfoMap>(managedMetadataFileName);
        }

        public override void OnTermStoreRunningStart(TermStore termStore)
        {
            termStore.Context.Load(termStore,
                s => s.Name);
            termStore.Context.ExecuteQuery();

            TermInfo termInfo = new TermInfo()
            {
                Type = Type.TermStore,
                Name = termStore.Name
            };

            // Write CSV
            managedMetadataFileWriter.WriteRecord(termInfo);
        }

        public override void OnTermGroupRunningStart(TermGroup termGroup)
        {
            termGroup.Context.Load(termGroup,
                s => s.Name);
            termGroup.Context.ExecuteQuery();

            TermInfo termInfo = new TermInfo()
            {
                Type = Type.TermGroup,
                Name = termGroup.Name
            };

            // Write CSV
            managedMetadataFileWriter.WriteRecord(termInfo);
        }

        public override void OnTermSetRunningStart(TermSet termSet)
        {
            termSet.Context.Load(termSet,
                s => s.Name);
            termSet.Context.ExecuteQuery();

            TermInfo termInfo = new TermInfo()
            {
                Type = Type.TermSet,
                Name = termSet.Name
            };

            // Write CSV
            managedMetadataFileWriter.WriteRecord(termInfo);
        }

        public override void OnTermRunningStart(Term term)
        {
            term.Context.Load(term,
                s => s.Name);
            term.Context.ExecuteQuery();

            TermInfo termInfo = new TermInfo()
            {
                Type = Type.Term,
                Name = term.Name
            };

            // Write CSV
            managedMetadataFileWriter.WriteRecord(termInfo);
        }
    }
}
