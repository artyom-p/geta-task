using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;

using Legacy.Core.PageTypes;
using Legacy.Core.Services;
using Legacy.Web.Templates.Base;
using Legacy.Web.Utilities;

namespace Legacy.Web.Templates.Pages
{
    public partial class ApplicationForm : TemplatePageBase<ApplicationFormPage>
    {
        private readonly HashSet<ContactPerson> _contactPersonList;
        
        protected const string[] CountyList = 
        {
             "", 
             "Nordland", 
             "Nord Trøndelag", 
             "Sør Trøndelag", 
             "Møre og Romsdal", 
             "Sogn og Fjordane", 
             "Hordaland", 
             "Rogaland", 
             "Vest Agder" 
        };

        protected HashSet<ContactPerson> ContactPersons
        {
            get
            {
                if (this._contactPersonList == null)
                {
                    this._contactPersonList = InitializeContactPersons();
                }

                return this._contactPersonList;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                return;
            }

            Ddl_County.DataSource = CountyList;
            Ddl_County.DataBind();
        }

        protected bool SendFormContentByEmail()
        {
            //TODO: implement builder pattern

            string subject = PropertyService.GetStringProperty(CurrentPage, "EmailSubject");
            string content = BuildEmailContent();
            string applicationReciever = GetEmailForMunicipality(Ddl_Municipality.SelectedValue);
            string applicationSender = Txt_Email.Text;

            MailMessage mailMessage = BuildMail(applicationSender, subject, content, applicationReciever, applicationReciever, GetAttachments());
            return SendMail(mailMessage, true);
        }

        #region Fill GUI controls

        /// <summary>
        /// Populate Ddl_Municipality with municipality from the given county
        /// </summary>
        /// <param name="county"></param>
        protected void PopulateMunicipalityList(string county)
        {
            Ddl_Municipality.Items.Clear();
            Ddl_Municipality.Items.Add(new ListItem("", ""));

            foreach (ContactPerson cp in this.ContactPersons)
            {
                if (cp.County != county)
                {
                    continue;
                }

                if (cp.Municipality == "mrHeroy")
                {
                    Ddl_Municipality.Items.Add(new ListItem("Herøy", cp.Municipality));
                }
                else
                {
                    Ddl_Municipality.Items.Add(new ListItem(cp.Municipality));
                }
            }
        }

        /// <summary>
        /// Creates as many FileUpload controls as configured on the page.
        /// </summary>
        private void BuildDynamicControls()
        {
            if (!pnlFileUpload.Visible)
            {
                return;
            }

            const string propertyName = "NumberOfFileUploads";
            //Create dummy datasource to display the correct number of FileUpload controls.
            if (CurrentPage.Property[propertyName].IsNull)
            {
                return;
            }

            int numberOfFiles = (int)CurrentPage.Property[propertyName].Value;
            if (numberOfFiles <= 0)
            {
                return;
            }

            var list = new List<int>();
            for (int i = 0; i < numberOfFiles; i++)
            {
                list.Add(i);
            }

            rptFileUpload.DataSource = list;
            rptFileUpload.DataBind();
        }
        #endregion

        #region Events

        /// <summary>
        /// Attachement button clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">e</param>
        protected void btnShowFileUpload_Click(object sender, EventArgs e)
        {
            pnlFileUpload.Visible = true;
            BuildDynamicControls();
            btnShowFileUpload.Visible = false;
        }

        /// <summary>
        /// Submit button clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">e</param>
        protected void Btn_SubmitForm_Click(object sender, EventArgs e)
        {
            // Server side validation, if javascript is disabled
            Page.Validate();

            if (!Page.IsValid)
            {
                return;
            }

            if (SendFormContentByEmail())
            {
                string receiptUrl = PropertyService.GetPageDataPropertyLinkUrl(CurrentPage, "FormReceiptPage");
                Response.Redirect(receiptUrl);
            }
            else
            {
                string errorUrl = PropertyService.GetPageDataPropertyLinkUrl(CurrentPage, "FormErrorPage");
                Response.Redirect(errorUrl);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the Ddl_County control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Ddl_County_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Ddl_County.SelectedValue.Equals(string.Empty))
            {
                PopulateMunicipalityList(Ddl_County.SelectedValue);
            }
            else
            {
                Ddl_Municipality.Items.Clear();
                Ddl_Municipality.DataBind();
            }
        }

        #endregion

        #region Email handling

        /// <summary>
        /// Builds the mail.
        /// </summary>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="content">The content.</param>
        /// <param name="fromAdress">From adress.</param>
        /// <param name="bccAddress">Bcc adress.</param>
        /// <param name="attachmentCol">The attachment col.</param>
        /// <returns></returns>
        protected MailMessage BuildMail(string toAddresses, string subject, string content, string fromAdress, string bccAddress, Attachment[] attachmentCol)
        {
            //From
            var mail = new MailMessage();

            //To
            toAddresses
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => mail.To.Add(a));

            mail.From = new MailAddress(fromAdress, fromAdress);
            mail.Subject = subject;
            mail.Body = content;

            if (!string.IsNullOrEmpty(bccAddress))
            {
                mail.Bcc.Add(bccAddress);
            }

            //Attachment
            if (attachmentCol != null)
            {
                attachmentCol
                    .Where(a => a != null)
                    .Select(a => mail.Attachments.Add(a));
            }

            return mail;
        }

        /// <summary>
        /// Sends an email with calendar event.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="isBodyHtml">if set to <c>true</c> [is body HTML].</param>
        /// <returns></returns>
        public bool SendMail(MailMessage mail, bool isBodyHtml)
        {
            if (mail.To.Count == 0 || string.IsNullOrEmpty(mail.From) || string.IsNullOrEmpty(mail.Subject))
            {
                return false;
            }

            foreach (MailAddress singleToAddress in mail.To)
            {
                if (!StringValidationUtil.IsValidEmailAddress(singleToAddress.Address))
                {
                    return false;
                }
            }

            mail.IsBodyHtml = isBodyHtml;

            try
            {
                using var smtp = new SmtpClient();
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a list of selected Attachments
        /// </summary>
        /// <returns></returns>
        private Attachment[] GetAttachments()
        {
            var attachmentList = new List<Attachment>();

            foreach (string postedInputName in Request.Files)
            {
                var postedFile = Request.Files[postedInputName];

                if (postedFile == null || postedFile.ContentLength == 0)
                {
                    continue;
                }
                
                string fileName = Path.GetFileName(postedFile.FileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                var newAttachment = new Attachment(postedFile.InputStream, fileName, postedFile.ContentType);
                attachmentList.Add(newAttachment);
            }

            return attachmentList.ToArray();
        }

        /// <summary>
        /// Builds the content of the email body
        /// </summary>
        /// <returns></returns>
        protected string BuildEmailContent()
        {
            const string SummaryStart = "<table>";
            const string SummaryEnd = "</table>";
            const string ContentStart = "<html>";
            const string ContentEnd = "</html>";
            const string LabelElementStart = "<tr><td><strong>";
            const string LabelElementEnd = "</strong></td>";
            const string ValueElementStart = "<td>";
            const string ValueElementEnd = "</td></tr>";
            const string LabelElementFullWidthStart = "<tr><td colspan=\"2\"><strong>";
            const string LabelElementFullWidthEnd = "</strong></td></tr>";
            const string ValueElementFullWidthStart = "<tr><td colspan=\"2\">";
            const string ValueElementFullWidthEnd = "</td></tr>";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ContentStart);
            stringBuilder.AppendLine(PropertyService.GetStringProperty(CurrentPage, "EmailHeader"));
            stringBuilder.AppendLine(SummaryStart);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/county") + LabelElementEnd + ValueElementStart + Ddl_County.SelectedValue + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/municipality") + LabelElementEnd + ValueElementStart + Ddl_Municipality.SelectedItem + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/applicator") + LabelElementEnd + ValueElementStart + Txt_Applicator.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/address") + LabelElementEnd + ValueElementStart + Txt_Address.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/postcode") + " / " + GetLanguageString("/applicationform/postarea") + LabelElementEnd + ValueElementStart + Txt_PostCode.Text + " " + Txt_PostArea.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/orgnobirthnumber") + LabelElementEnd + ValueElementStart + Txt_OrgNoBirthNumber.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/contactperson") + LabelElementEnd + ValueElementStart + Txt_ContactPerson.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/phone") + LabelElementEnd + ValueElementStart + Txt_Phone.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/email") + LabelElementEnd + ValueElementStart + Txt_Email.Text + ValueElementEnd);
            stringBuilder.AppendLine(LabelElementFullWidthStart + GetLanguageString("/applicationform/description") + LabelElementFullWidthEnd + ValueElementFullWidthStart + Txt_Description.Text + ValueElementFullWidthEnd);
            stringBuilder.AppendLine(LabelElementFullWidthStart + GetLanguageString("/applicationform/financeplan") + LabelElementFullWidthEnd + ValueElementFullWidthStart + Txt_FinancePlan.Text + ValueElementFullWidthEnd);
            stringBuilder.AppendLine(LabelElementFullWidthStart + GetLanguageString("/applicationform/businessdescription") + LabelElementFullWidthEnd + ValueElementFullWidthStart + Txt_BusinessDescription.Text + ValueElementFullWidthEnd);
            stringBuilder.AppendLine(LabelElementStart + GetLanguageString("/applicationform/applicationAmount") + LabelElementEnd + ValueElementStart + Txt_ApplicationAmount.Text + ValueElementEnd);
            stringBuilder.AppendLine(SummaryEnd);
            stringBuilder.AppendLine(PropertyService.GetStringProperty(CurrentPage, "EmailFooter"));
            stringBuilder.AppendLine(ContentEnd);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the email address or the contact person for provided municipality (kommune)
        /// </summary>
        /// <param name="municipality"></param>
        /// <returns></returns>
        protected string GetEmailForMunicipality(string municipality)
        {
            foreach (ContactPerson cp in this.ContactPersons)
            {
                if (cp.Municipality.Equals(municipality, StringComparison.InvariantCultureIgnoreCase))
                {
                    return cp.Email;
                }
            }

            return null;
        }

        #endregion

        #region Language handling
        /// <summary>
        /// Returns the current language string for a specified xml language file entry.
        /// </summary>
        /// <param name="xmlPath">The path to the string in the xml file.</param>
        /// <returns></returns>
        protected static string GetLanguageString(string xmlPath)
        {
            return EPiServer.Core.LanguageManager.Instance.Translate(xmlPath, GetCurrentLanguage());
        }

        /// <summary>
        /// Returns the current language as a two letter code (no or en for instance).
        /// </summary>
        /// <returns></returns>
        protected static string GetCurrentLanguage()
        {
            return EPiServer.Globalization.ContentLanguage.PreferredCulture.Name;
        }
        #endregion

        #region ContactPerson list initialization
        private void InitializeContactPersons()
        {
            this._contactPersonList = new List<ContactPerson>
            {
                new ContactPerson("Sørfold", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Gildeskål", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Rødøy", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Dønna", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Herøy", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Alstahaug", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Brønnøy", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Sømna", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Leka", "Nord Trøndelag", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Nærøy", "Nord Trøndelag", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Meløy", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Høylandet", "Nord Trøndelag", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Bodø", "Nordland", "Kjell.Stokbakken@Legacy.com"),
                new ContactPerson("Fosnes", "Nord Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Flatanger", "Nord Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Osen", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Frøya", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Hitra", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Smøla", "Møre og Romsdal", "knut.utheim@Legacy.com"),
                new ContactPerson("Averøy", "Møre og Romsdal", "knut.utheim@Legacy.com"),
                new ContactPerson("Roan", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Snillfjord", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("Aure", "Møre og Romsdal", "knut.utheim@Legacy.com"),
                new ContactPerson("Bjugn", "Sør Trøndelag", "knut.utheim@Legacy.com"),
                new ContactPerson("mrHeroy", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Volda", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Vanylven", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Selje", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Vågsøy", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Bremanger", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Ørsta", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Ulstein", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Flora", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Leikanger", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Høyanger", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Fjaler", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Solund", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Hyllestad", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Gulen", "Sogn og Fjordane", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Ålesund", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Aukra", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Fræna", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Haram", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Giske", "Møre og Romsdal", "Per-Roar.Gjerde@Legacy.com"),
                new ContactPerson("Askøy", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Fjell", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Sund", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Etne", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Jondal", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Kvinnherad", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Tysvær", "Rogaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Vindafjord", "Rogaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Finnøy", "Rogaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Hjelmeland", "Rogaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Flekkefjord", "Vest Agder", "astrid.sande@Legacy.com"),
                new ContactPerson("Masfjorden", "Hordaland", "astrid.sande@Legacy.com"),
                new ContactPerson("Øygarden", "Hordaland", "astrid.sande@Legacy.com")
            };
        }
        #endregion
    }
}