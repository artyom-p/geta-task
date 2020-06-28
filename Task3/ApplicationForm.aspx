<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/MasterPages/MasterPage.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeBehind="ApplicationForm.aspx.cs" Inherits="Legacy.Web.Templates.Pages.ApplicationForm" %>
<%@ Import Namespace="Legacy.Core.Services" %>

<%@ Register TagPrefix="Legacy" TagName="PrinterfriendlyLink"  Src="~/Templates/PageContentUnits/PrinterfriendlyLink.ascx"  %>
<%@ Register TagPrefix="Legacy" TagName="MainBody" Src="~/Templates/PageContentUnits/MainBody.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="MainIntro" Src="~/Templates/PageContentUnits/MainIntro.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="PageName" Src="~/Templates/PageContentUnits/PageName.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="Backlink" Src="~/Templates/PageContentUnits/Backlink.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="BottomImage" Src="~/Templates/PageContentUnits/BottomImage.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="PageInfo"  Src="~/Templates/PageContentUnits/PageInfo.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="ArticleLanguageSelector" Src="~/Templates/Units/ArticleLanguageSelector.ascx" %>
<%@ Register TagPrefix="Legacy" TagName="RightContentHeadingAndTextbox"      Src="~/Templates/PageContentUnits/RightContentHeadingAndTextbox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashedline"><!-- ---></div>
    <div class="maincontent">
        <Legacy:PageName id="PageName1" runat="server" />
        <Legacy:Backlink id="Backlink1" runat="server" />

        <Legacy:MainIntro id="MainIntro1" runat="server" />
        <Legacy:MainBody id="MainBody1" runat="server" />
        <div class="vspacer10"></div>
        
        <div id="applicationForm">
            <div><EPiServer:Translate text="/applicationform/county" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Ddl_County" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:DropDownList runat="server" ID="Ddl_County" AutoPostBack="true" OnSelectedIndexChanged="Ddl_County_SelectedIndexChanged"></asp:DropDownList></div>
            
            <div><EPiServer:Translate text="/applicationform/municipality" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Ddl_Municipality" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:DropDownList runat="server" ID="Ddl_Municipality"></asp:DropDownList></div>
            
            <div><EPiServer:Translate text="/applicationform/applicator" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_Applicator" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_Applicator" /></div>
            
            <div><EPiServer:Translate text="/applicationform/address" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_Address" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_Address" /></div> 
            
            <div><EPiServer:Translate text="/applicationform/postcode" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_PostCode" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /><asp:RegularExpressionValidator runat="server" CssClass="warning" ControlToValidate="Txt_PostCode" ValidationExpression="\d{4}" ErrorMessage='<%# GetLanguageString("/applicationform/illegalvalue") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_PostCode" /></div>    
            
            <div><EPiServer:Translate text="/applicationform/postarea" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_PostArea" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_PostArea" /></div>  
            
            <div><EPiServer:Translate text="/applicationform/orgnobirthnumber" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_OrgNoBirthNumber" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /><asp:RegularExpressionValidator runat="server" CssClass="warning" ControlToValidate="Txt_OrgNoBirthNumber" ValidationExpression="\d{6,11}" ErrorMessage='<%# GetLanguageString("/applicationform/illegalvalue") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_OrgNoBirthNumber" /></div>         
            
            <div><EPiServer:Translate text="/applicationform/contactperson" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_ContactPerson" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_ContactPerson" /></div>         
            
            <div><EPiServer:Translate text="/applicationform/phone" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_Phone" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /><asp:RegularExpressionValidator runat="server" CssClass="warning" ControlToValidate="Txt_Phone" ValidationExpression="\d{8}" ErrorMessage='<%# GetLanguageString("/applicationform/illegalvalue") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_Phone" /></div>         
            
            <div><EPiServer:Translate text="/applicationform/email" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_Email" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /><asp:RegularExpressionValidator runat="server" CssClass="warning" ControlToValidate="Txt_Email" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage='<%# GetLanguageString("/applicationform/illegalvalue") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_Email" /></div>
            
            <div><EPiServer:Translate text="/applicationform/description" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_Description" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_Description" TextMode="MultiLine" Rows="5" /></div>      
            
            <div><EPiServer:Translate text="/applicationform/financeplan" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_FinancePlan" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_FinancePlan" TextMode="MultiLine" Rows="5" /></div> 
            
            <div><EPiServer:Translate text="/applicationform/businessdescription" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_BusinessDescription" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_BusinessDescription" TextMode="MultiLine" Rows="5" /></div>                     
           
            <div><EPiServer:Translate text="/applicationform/applicationAmount" runat="server" /> <asp:RequiredFieldValidator runat="server" CssClass="warning" ControlToValidate="Txt_ApplicationAmount" ErrorMessage='<%# GetLanguageString("/applicationform/requiredfield") %>'></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" CssClass="warning" ControlToValidate="Txt_ApplicationAmount" ValidationExpression="\d+" ErrorMessage='<%# GetLanguageString("/applicationform/illegalvalue") %>' /></div>
            <div><asp:TextBox runat="server" ID="Txt_ApplicationAmount" /></div>         

            <div>
                <asp:Button ID="btnShowFileUpload" Text='<%$ Resources: EPiServer, applicationform.attachdocuments %>' CausesValidation="false" 
                    runat="server" OnClick="btnShowFileUpload_Click" Visible='<%# Int32.Parse(PropertyService.GetStringProperty(CurrentPage, "NumberOfFileUploads")) > 0 %>'></asp:Button>
                <asp:Panel ID="pnlFileUpload" runat="server" Visible="false">
                    <EPiServer:Translate text="/applicationform/attachments" runat="server" />
                    <asp:Repeater ID="rptFileUpload" runat="server">
                        <ItemTemplate>
                            <div class="clearfix">
                                <label class="cabinet">
                                    <asp:FileUpload ID="FileUpload1" CssClass="file" runat="server" />
                                    <span><EPiServer:Translate text="/applicationform/upload" runat="server" /></span>
                                </label>
                                <span class="filename"></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </div>      
            <div class="clearfix">
                <asp:Button runat="server" CssClass="formSubmitter" Text='<%$ Resources: EPiServer, applicationform.sendapplication %>' ID="Btn_SubmitForm" OnClick="Btn_SubmitForm_Click" />
            </div>
            <style type="text/css">
                #applicationForm .warning { 
                	float: right; 
                }
                #applicationForm select { 
                	min-width: 50%; 
                	height: 19px; 
                }
                #applicationForm input[type=text] { 
                	min-width: 50%; 
                	height: 15px; 
                }
                #applicationForm textarea { 
                	width: 100%; 
                	max-width: 100%; 
                	min-width: 100%; 
                }
                
                #applicationForm select, 
                #applicationForm input[type=text], 
                #applicationForm textarea {
                	margin-bottom: 10px;
                }
                
                #applicationForm input.formSubmitter { 
                	margin-top: 10px; 
                	float: right; 
                }
                #applicationForm input[type=submit] { 
                	border: none; 
                	background-color: #9D948B; 
                	color: white; 
                	margin: 0; 
                	padding: 1px 4px 2px 4px; 
                	border-radius: 2px; 
                }
                #applicationForm input[type=submit]:hover { 
                	cursor: pointer; 
                	background-color: #69645F; 
                }
                
                #applicationForm select, 
                #applicationForm input[type=text],
                #applicationForm textarea {
                	font-family: Arial, Helvetica, sans-serif;
                	border: 1px solid #9D948B;
                    font-size: 11px;
                }
                
                .SI-FILES-STYLIZED label.cabinet
                {
                    width: 80px;
                    height: 22px;
                    background-color: #9D948B;
                    margin-bottom: 5px;
                    float: left;
                    overflow: hidden;
                    cursor: pointer;
                    border-radius: 2px;
                    color: white;
                    
                    line-height: 22px;
                    font-size: 13.3px;
                }
                .SI-FILES-STYLIZED label.cabinet:hover {
                	background-color: #69645F;
                	cursor: pointer;
                }
                .SI-FILES-STYLIZED label.cabinet span {
                	display: block;
                	margin-top: -22px;
                	text-align: center;
                }
                .SI-FILES-STYLIZED label.cabinet input.file:hover { 
                	cursor: pointer;
                	background: none;
                }

                .SI-FILES-STYLIZED label.cabinet input.file
                {
                	background: none;
                    position: relative;
                    height: 22px;
                    width: auto;
                    opacity: 0;
                    -moz-opacity: 0;
                    filter:progid:DXImageTransform.Microsoft.Alpha(opacity=0);
                }
                .SI-FILES-STYLIZED span.filename {
                	float: left;
                    font-size: 13.3px;
                    height: 22px;
                    line-height: 22px;
                    margin: 0 0 5px 5px;
                }           	
            </style>
            <script type="text/javascript" src="/Scripts/jquery-v.1.4.4.js"></script>
            <script type="text/javascript" src="/Scripts/si.files.js"></script>
            <script type="text/javascript" src="/Scripts/jquery.scrollTo-min.js"></script>
            <script type="text/javascript">
                $(document).ready(function () {
                    SI.Files.stylizeAll();

                    scrollToFirstError();

                    $('.formSubmitter').bind('click', function () {
                        scrollToFirstError();
                    });

                    $('#applicationForm input[type=file]').change(function () {
                        var text = $(this).val();
                        $(this).parent().siblings('.filename').text($(this).val());
                    });
                });

                function scrollToFirstError() {
                    $.each($('.warning'), function () {
                        if ($(this).css('visibility') != 'hidden') {
                            $.scrollTo($(this));
                            return false;
                        }
                    });
                }
            </script>
        </div>
        
        <div class="vspacer10"></div>
        <Legacy:PageInfo id="PageInfo1" runat="server" />
        <Legacy:PrinterfriendlyLink id="PrinterfriendlyLink1" runat="server" />
        <div class="clearboth"></div>
        <div class="vspacer16"></div>
        <Legacy:BottomImage id="BottomImage1" runat="server" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="RightContent" runat="server">
    <Legacy:RightContentHeadingAndTextbox runat="server" />
</asp:Content>