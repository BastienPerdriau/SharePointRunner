using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;

namespace SharePointRunner.LauncherV1
{
    public class TermInfo
    {
        public string TermStore { get; set; }

        public string TermGroup { get; set; }

        public string TermSet { get; set; }

        public string Term { get; set; }
    }

    public class ManagedMetadataReceiver : Receiver
    {
        public override void OnTenantRunningStart(Tenant tenant)
        {
            // TODO V2 ManagedMetadataReceiver

            tenant.Context.Load(tenant,
                t => t.RootSiteUrl);
            tenant.Context.ExecuteQuery();

            string managedMetadataFileName = $"AuditManagedMaetadata-{tenant.RootSiteUrl}--{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";

            TaxonomySession taxSession = TaxonomySession.GetTaxonomySession(tenant.Context);

            tenant.Context.Load(taxSession,
                session => session.TermStores.Include(
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name,
                        group => group.TermSets.Include(
                            set => set.Name,
                            set => set.Terms.Include(
                                term => term.Name,
                                term => term.Terms)))));
            tenant.Context.ExecuteQuery();
        }
    }
}
