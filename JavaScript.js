
var retrieveFilter = "?$select=new_MemberCategoryId&$filter=ContactId eq guid'" + customer[0].id + "'";
var memberCategory = RetrieveRecordSync(retrieveFilter, "ContactSet");
if (!memberCategory) {
    Xrm.Page.getAttribute("new_membercategoryid").setValue([{ id: memberCategory.Id, name: memberCategory.Name, entityType: memberCategory.LogicalName }]);
}

function RetrieveRecordSync(filter, entitySet) {
    var memberCategory = null;
    var crmServerUrl = Xrm.Page.context.getClientUrl();
    var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
    var ODATA_Final_url = crmServerUrl + ODATA_ENDPOINT + entitySet + filter;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: false,
        url: ODATA_Final_url,
        beforeSend: function (XMLHttpRequest) {   
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data != null && data.d != null && data.d.results.length > 0) {
                memberCategory = data.d.results[0];
            }
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            alert("Error: Failed to set primary customer site.");
        }
    });
    return memberCategory;
}
