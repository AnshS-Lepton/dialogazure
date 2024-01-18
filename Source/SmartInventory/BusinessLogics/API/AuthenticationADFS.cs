using Models;
using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using Thinktecture.IdentityModel.WSTrust;

namespace BusinessLogics.API
{
    public  class AuthenticationADFS
    {
        public static ADFSDetail AuthenticateADFS(ADFSInput objADFSInput)
        {

            WSTrustChannelFactory factory = null;
            ADFSDetail ADFSDetail = null;
            try
            {

                factory = new WSTrustChannelFactory(
                new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(objADFSInput.ADFSEndPoint));

                factory.TrustVersion = TrustVersion.WSTrust13;

                // Username and Password here...             
                if (!string.IsNullOrEmpty(objADFSInput.ADFSAutheticationBasedOn)
                    && objADFSInput.ADFSAutheticationBasedOn.ToUpper() == "USERNAME")
                {
                    factory.Credentials.UserName.UserName = objADFSInput.ADFSUserNamePreFix + objADFSInput.user_name;
                }
                else
                {
                    factory.Credentials.UserName.UserName = objADFSInput.user_email;
                }
                factory.Credentials.UserName.Password = objADFSInput.password;

                RequestSecurityToken rst = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = new EndpointReference(objADFSInput.ADFSRelPartyUri),
                    KeyType = KeyTypes.Bearer,
                };

                IWSTrustChannelContract channel = factory.CreateChannel();
                RequestSecurityTokenResponse rstr;
                SecurityToken token = channel.Issue(rst, out rstr);

                ADFSDetail = new ADFSDetail() { tokenId = token.Id, validFrom = token.ValidFrom, validTo = token.ValidTo };

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ID3242"))
                    ADFSDetail = new ADFSDetail() { errorMsg = "Invalid ADFS username or password" };
                else if (ex.Message.Contains("ID3082"))
                    ADFSDetail = new ADFSDetail() { errorMsg = "Invalid ADFS uri configuration" };
            }
            finally
            {
                if (factory != null)
                {
                    try
                    {
                        factory.Close();
                    }
                    catch (CommunicationObjectFaultedException)
                    {
                        factory.Abort();
                    }
                }
            }
            return ADFSDetail;


        }
    }
}
