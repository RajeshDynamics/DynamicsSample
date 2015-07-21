using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;

namespace XYZ.CRM.Plugins
{
    public class UpdatePlanDetails : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            Entity entity = null;
            string strMessage = string.Empty;

            // Check if the input parameters property bag contains a target
            // of the update operation and that target is of type Entity.
            if (context.MessageName.ToLower() == "create" && context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target business entity from the input parmameters.
                entity = (Entity)context.InputParameters["Target"];
                // Verify that the entity represents an opportunity.
                if (entity.LogicalName != "opportunity") { return; }

                try
                {
                    Guid memberCategoryId = entity.Attributes.Contains("new_membercategoryid") ? ((EntityReference)entity.Attributes["new_membercategoryid"]).Id : null;
                    if (memberCategoryId)
                    {
                        Entity priorityLevelDetails = service.Retrieve("new_prioritylevel", memberCategoryId, new ColumnSet(true));
                        if (priorityLevelDetails)
                        {
                            Guid slaDays = priorityLevelDetails.Contains("new_sladays") ? priorityLevelDetails["new_sladays"].toString() : string.Empty;
                            bool premiumClient = priorityLevelDetails.Contains("new_level") ? (bool)priorityLevelDetails["new_level"].toString() : false;
                            entity.Attributes.Add("new_sladays", slaDays);
                            entity.Attributes.Add("new_level", premiumClient);
                        }
                    }

                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    Logger.Log(ex);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }
    }
}
