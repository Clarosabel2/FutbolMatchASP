﻿using BE;
using BLL;
using SERVICES;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI.Webforms
{
    public partial class frmRegister : System.Web.UI.Page
    {
        //Validar Campos
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RolesLoad();
                LanguageLoad();
                //Modificacion de usuario seleccionado
                if (Request.QueryString["username"] != null && Request.QueryString["modifyUser"] == null)
                {
                    TextBoxPassword.Visible = false;

                    LabelPassword.Visible = false;
                    LabelBlocked.Visible = true;
                    LabelRemoved.Visible = true;

                    CheckBoxBlocked.Visible = true;
                    CheckBoxRemoved.Visible = true;

                }
                //Modificacion de usuario seleccionado
                if (Request.QueryString["modifyUser"] != null)
                {
                    TextBoxPassword.Visible = true;

                    LabelPassword.Visible = true;
                    LabelBlocked.Visible = false;
                    LabelRemoved.Visible = false;
                    LabelRoles.Visible = false;

                    DropDownListRoles.Visible = false;

                    CheckBoxBlocked.Visible = false;
                    CheckBoxRemoved.Visible = false;

                }
                //Modificacion mis datos
                if (Request.QueryString["username"] != null)
                {
                    SetUserData(Request.QueryString["username"].ToString());
                    ButtonRegister.Text = "Modificar";
                    TextBoxUsername.ReadOnly = true;
                    LabelPassword.Visible = false;
                    TextBoxPassword.Visible = false;
                    SectionPasswordMod.Visible = true;
                }
            }
        }

        private void SetUserData(string username)
        {
            BE_User user = BLL_User.GetUserByUsername(username);
            TextBoxName.Text = user.Name;
            TextBoxLastname.Text = user.Lastname;
            TextBoxUsername.Text = user.Username;
            TextBoxPhone.Text = user.Phone;
            TextBoxEmail.Text = user.Email;
            DropDownListRoles.SelectedValue = user.Role;
            CheckBoxBlocked.Checked = user.Blocked;
            CheckBoxRemoved.Checked = user.Removed;
        }
        private void LanguageLoad()
        {
            DropDownListLanguage.DataTextField = "languageDisplay";
            DropDownListLanguage.DataValueField = "idLanguage";
            DropDownListLanguage.DataSource = BLL_Language.GetLanguages();
            DropDownListLanguage.DataBind();
        }

        private void RolesLoad()
        {
            DropDownListRoles.DataTextField = "roleName";
            DropDownListRoles.DataValueField = "idRole";
            DropDownListRoles.DataSource = BLL_Role.GetRoles();
            DropDownListRoles.DataBind();
        }
        private void EstablishmentsLoad()
        {
            DataTable establishments = BLL_Establishment.GetEstablishments();
            DataTable filteredTable = establishments.DefaultView.ToTable(false, "idEstablishment", "establishmentName");
            CheckBoxListEstablishments.Items.Clear();
            foreach (DataRow item in filteredTable.Rows)
            {
                CheckBoxListEstablishments.Items.Add(new ListItem(item["establishmentName"].ToString(), item["idEstablishment"].ToString()));
            }
        }
        private bool ValidateFields()   
        {
            if (TextBoxEmail.Text == "" || TextBoxUsername.Text == "" || TextBoxName.Text == "" || TextBoxLastname.Text == "")
            {
                WebformMessage.ShowMessage("Debe completar los campos solicitados", this);
                return false;
            }
            return true;
        }
        private bool ComparePassword()
        {
            if (Encrpyt.HashValue(TextBoxCurrentPass.Text) == SessionManager.GetInstance.User.Password)
            {
                if (TextBoxNwPass.Text == TextBoxConfirmPass.Text)
                {
                    return true;
                }
                else { WebformMessage.ShowMessage("Contraseñas no coinciden", this); }

            }
            else { WebformMessage.ShowMessage("Contraseña Incorrecta", this); }

            return false;
        }

        protected void ButtonRegister_Click(object sender, EventArgs e)
        {
            //Falta modificacion de establecimiento
            if (ValidateFields())
            {
                BE_User user = new BE_User(
                TextBoxUsername.Text,
                Encrpyt.HashValue(TextBoxPassword.Text),
                TextBoxName.Text,
                TextBoxLastname.Text,
                TextBoxEmail.Text,
                TextBoxPhone.Text,
                DropDownListRoles.SelectedItem.Text,
                Convert.ToInt32(DropDownListLanguage.SelectedValue),
                CheckBoxBlocked.Checked,
                CheckBoxRemoved.Checked);

                if (Request.QueryString["username"] != null)
                {
                    if (Request.QueryString["modifyUser"] != null)
                    {
                        if (CheckBoxModPass.Checked)
                        {
                            if (ComparePassword())
                            {
                                user.Password = Encrpyt.HashValue(TextBoxConfirmPass.Text);
                                BLL_User.UpdateMyAccount(user);
                                Response.Redirect("frmMyAccount.aspx");

                            }
                        }
                        else
                        {
                            BLL_User.UpdateUser(user);
                            Response.Redirect("frmMyAccount.aspx");
                        }

                    }
                    else
                    {
                        BLL_User.UpdateUser(user);
                        Response.Redirect("frmUsers.aspx");
                    }

                }
                else
                {
                    if (BLL_User.InsertUser(user))
                    {
                        //asocia usuario con estableclimiento
                        if (DropDownListRoles.SelectedItem.Text != "WEBMASTER")
                        {
                            try
                            {
                                bool isAnySelected = false;
                                foreach (ListItem item in CheckBoxListEstablishments.Items)
                                {
                                    if (item.Selected)
                                    {
                                        isAnySelected = true;
                                        BLL_Establishment.SetUserEstablishment(user.Username, item.Text);
                                    }
                                }
                                if (!isAnySelected)
                                {
                                    throw new Exception("No se seleccionó ningún establecimiento.");
                                }
                                Response.Redirect("frmUsers.aspx");

                            }
                            catch (Exception ex)
                            {
                                Response.Write(ex.Message);
                            }
                        }
                        else
                        {
                            Response.Redirect("frmUsers.aspx");
                        }

                    }
                    else
                    {
                        WebformMessage.ShowMessage("Complete todos los campos", this);
                    }
                }
            }

        }

        protected void CheckBoxModPass_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxModPass.Checked)
            {
                PanelReqModPass.Visible = true;
            }
            else { PanelReqModPass.Visible = false; }
        }

        protected void DropDownListRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //POR AHORA
            if (DropDownListRoles.SelectedItem.Text == "ADMIN")
            {
                PanelEstablishments.Visible = true;
                EstablishmentsLoad();
            }
            else
            {
                PanelEstablishments.Visible = false;
            }
        }
    }
}