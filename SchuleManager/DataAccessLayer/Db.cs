using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web;
using Dapper;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.DataAccessLayer
{
    ///<summary>
    ///Db class contains methods used to perform CRUD operations on the database
    ///</summary>
    public class Db
    {

        //private static readonly CreateErrorLogs Clf = new CreateErrorLogs();
        private static CreateErrorLogs _clf;
        private static readonly string ErrorLogPath = Path.Combine(HttpContext.Current.Server.MapPath("~/ErrorLogs/"));

        public string Con;
        
        public Db(string entityName, string username = "")
        {
            Con = entityName;

            _clf = new CreateErrorLogs(username + "_db_access_log_");
        }

        ///<summary>
        ///This method returns the details of system at login
        ///</summary>
        /// <returns>returns a datatable of the system details at Login</returns>
        /// 
        public DataTable GetSystemStatusDetails()
        {

            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSystemStausDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemStatusDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemStatusDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

              }
            }
            return dt;
        }

        ///<summary>
        ///This method returns the details of a system user from the database
        ///</summary>
        /// <param name="username">Login ID of System User</param>
        /// <param name="password">Encrypted Password of System User</param>
        /// <returns>returns a dataset of the System user's details</returns>
        /// 
        public DataSet GetUserLoginCredentials(string username, string password)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetUserLoginCredentials", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@UserPassword", password);

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(ds);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return ds;
        }

        #region System Security DB Access Methods

        public DataTable GetUserDetails(string username)
        {

            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetUserDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName", username);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetRoleDetails(string roleCode)
        {

            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetRoleDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public int AddEditSystemUsers(SystemUsers user, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();

                    const string cashGlCheckQuery = "SELECT COUNT(*) FROM t_SystemUsers WHERE CashierGL=@CashierGL";

                    var cmd1 = new SqlCommand(cashGlCheckQuery, conn);

                    cmd1.Parameters.AddWithValue("@CashierGL", user.CashierGl);

                    var glAssigned = (int)cmd1.ExecuteScalar() > 0;

                    if (glAssigned) return -10;
                    using (var cmd2 = new SqlCommand("sp_AddEditSystemUsers", conn))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        //input data
                        cmd2.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd2.Parameters.AddWithValue("@RoleCode", user.RoleCode);
                        cmd2.Parameters.AddWithValue("@UserPassword", EncryptionLibrary.EncryptText(user.UserPassword));
                        cmd2.Parameters.AddWithValue("@SurName", user.SurName);
                        cmd2.Parameters.AddWithValue("@OtherNames", user.OtherNames);
                        cmd2.Parameters.AddWithValue("@EmailId", user.EmailId.ToLower());
                        cmd2.Parameters.AddWithValue("@PhoneNo1", user.PhoneNo1);
                        cmd2.Parameters.AddWithValue("@IsCashier", user.IsCashier);
                        cmd2.Parameters.AddWithValue("@CashierGL", user.CashierGl);
                        cmd2.Parameters.AddWithValue("@PasswordNeverExpires", user.PasswordNeverExpires);
                        cmd2.Parameters.AddWithValue("@MustchangePassword", user.MustChangePassword);
                        cmd2.Parameters.AddWithValue("@IsDisabled", user.IsDisabled);
                        cmd2.Parameters.AddWithValue("@Enterer", enterer);
                        cmd2.Parameters.AddWithValue("@Action", action);

                        result = cmd2.ExecuteNonQuery();
                       
                    }
                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditSystemUsers method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditSystemUsers method: " + ex);
                }
                finally
                {
                    conn.Close();
                }
            }
            return result;
        }

        public int SuperviseSystemUsers(string username, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {

                using (var cmd = new SqlCommand("sp_AddEditSystemUsers", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        //input data
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at SuperviseSystemUsers method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at SuperviseSystemUsers method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public DataTable GetRoles()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetRoles", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoles method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoles method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetRoleMembers(string roleCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetRoleMembers", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleMembers method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleMembers method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public int AddEditRoles(SystemRoles role, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();

                    const string cashGlCheckQuery = "SELECT COUNT(*) FROM t_Roles WHERE RoleCode=@RoleCode";

                    var cmd1 = new SqlCommand(cashGlCheckQuery, conn);

                    cmd1.Parameters.AddWithValue("@RoleCode", role.RoleCode);

                    var codeExists = (int)cmd1.ExecuteScalar() > 0;

                    if (codeExists) return -10;

                    using (var cmd2 = new SqlCommand("sp_AddEditRoles", conn))
                    {
                            cmd2.CommandType = CommandType.StoredProcedure;
                            //input data
                            cmd2.Parameters.AddWithValue("@RoleCode", role.RoleCode);
                            cmd2.Parameters.AddWithValue("@Description", role.Description);
                            cmd2.Parameters.AddWithValue("@Deleted", role.Deleted);
                            cmd2.Parameters.AddWithValue("@Enterer", enterer);
                            cmd2.Parameters.AddWithValue("@Action", action);

                            result = cmd2.ExecuteNonQuery();
                    }
                    
                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditRoles method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditRoles method: " + ex);
                }
                finally
                {
                    conn.Close();
                }

            }
            return result;
        }

        public DataTable GetModuleTreeData(string roleCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSystemModuleTreeData", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModuleTreeData method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModuleTreeData method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int GetModuleParentId(int moduleId)
        {
            var id = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT dbo.fn_GetModuleParentId(@Id)", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Id", moduleId);
                        id = (int)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModuleParentId method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModuleParentId method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return id;
        }

        public DataTable GetModules()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetModules", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModules method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetModules method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetRoleRights(string roleCode, int subModuleId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                using (var cmd = new SqlCommand("sp_GetRoleRights", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                        cmd.Parameters.AddWithValue("@SubModuleId", subModuleId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleRights method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleRights method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int UpdateRoleRights(string roleCode, DataTable roleRights)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateRoleRights", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                        cmd.Parameters.AddWithValue("@RoleRights", roleRights);
                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateRoleRights method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateRoleRights method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }

        public DataTable GetDataEntryRights(string roleCode, int menuId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                using (var cmd = new SqlCommand("sp_GetDataEntryRights", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                        cmd.Parameters.AddWithValue("@MenuId", menuId);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetDataEntryRights method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetDataEntryRights method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public void UpdateDataEntryRights(DataEntryRights der)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateDataEntryRights", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@RoleCode", der.RoleCode);
                        cmd.Parameters.AddWithValue("@MenuId", der.MenuId);
                        cmd.Parameters.AddWithValue("@CanAdd", der.CanAdd);
                        cmd.Parameters.AddWithValue("@CanEdit", der.CanEdit);
                        cmd.Parameters.AddWithValue("@CanDelete", der.CanDelete);
                        cmd.Parameters.AddWithValue("@PostingLimit", der.PostingLimit);
                        cmd.Parameters.AddWithValue("@CanSupervise", der.CanSupervise);
                        cmd.Parameters.AddWithValue("@SupervisionLimit", der.SupervisionLimit);

                        cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateDataEntryRights method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateDataEntryRights method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        #endregion

        #region System Settings DB Access Methods

        public SystemSettings GetSchoolInfo()
        {
            SystemSettings settings = null;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                try
                {
                    conn.Open();
                    var param = new DynamicParameters();
                    settings = conn.Query<SystemSettings>("sp_GetSchoolInfo", param, null, true, 0,
                        CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSchoolInfo method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSchoolInfo method: " + ex);
                }
                finally
                {
                    conn.Close();
                }
            }
            return settings;
        }

        public int UpdateSchoolInfo(SystemSettings schoolInfo, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateSchoolInfo", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@PAddress", schoolInfo.PAddress);
                        cmd.Parameters.AddWithValue("@BoxAddress", schoolInfo.BoxAddress);
                        cmd.Parameters.AddWithValue("@Email1", schoolInfo.Email1);
                        cmd.Parameters.AddWithValue("@Email2", schoolInfo.Email2);
                        cmd.Parameters.AddWithValue("@Phone1", schoolInfo.Phone1);
                        cmd.Parameters.AddWithValue("@Phone2", schoolInfo.Phone2);
                        cmd.Parameters.AddWithValue("@Website", schoolInfo.Website);
                        cmd.Parameters.AddWithValue("@TagLine", schoolInfo.TagLine);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateSchoolInfo method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateSchoolInfo method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }

        public DataTable GetSystemHolidays()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSystemHolidays", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemHolidays method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemHolidays method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public int AddEditSystemHolidays(SystemHolidays holiday, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditSystemHolidays", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@HolidayDate", holiday.HolidayDate);
                        cmd.Parameters.AddWithValue("@Remarks", holiday.Remarks);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditSystemHolidays method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditSystemHolidays method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        #endregion

        ///<summary>
        ///This method checks with username exists in the database
        ///</summary>
        /// <param name="username">Login ID of System User</param>
        /// <returns>returns true or false</returns>
        public bool UsernameExists(string username)
        {
            var userexists = false;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();

                    var cmd = new SqlCommand("select * from t_SystemUsers where UserName=@UserName", conn);
                    cmd.Parameters.AddWithValue("@UserName", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        userexists = reader.Read();
                    }
                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UsernameExists method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UsernameExists method: " + ex);
                }
                finally
                {
                    conn.Close();
                }
            }
            return userexists;
        }
        
        public bool UpdateUserPassword(int userId, string newPassword)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_ChangePassword", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        //input branch code
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@NewPswd", newPassword);
                        cmd.Parameters.AddWithValue("@ChangeDate", DateTime.Now);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateUserPassword method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateUserPassword method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return result > 0;
                }
            }
        }

        public string GetUserPassword(int userid)
        {
            string password = null;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();
                    var command = new SqlCommand("Select UserPassword from t_SystemUsers where UserId=@UserId", conn);
                    command.Parameters.AddWithValue("@UserId", userid);
                    using (var reader = command.ExecuteReader())
                    {
                        password = reader["UserPassword"].ToString();
                    }
                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserPassword method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUserPassword method: " + ex);
                }
                finally
                {
                    conn.Close();
                }

            }
            return password;
        }

        public DataTable GetSystemUsers()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSystemUsers", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemUsers method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSystemUsers method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetAccessRights(string roleCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAccessRights", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAccessRights method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAccessRights method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public SystemRoles GetRoleDetails(int roleId)
        {
            SystemRoles result =  null;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();
                    //input data
                    var param = new DynamicParameters();
                    param.Add("@RoleId", roleId);
                    result = conn.Query<SystemRoles>("sp_GetRoleDetails", param, null, true, 0,
                        CommandType.StoredProcedure).SingleOrDefault();

                }
                catch (SqlException ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleDetails method: " + ex);
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetRoleDetails method: " + ex);
                }
                finally
                {
                    conn.Close();
                }

            }
            return result;
        }
        
        public int AddEditPhotos(string clientId, string photoData, string photoExt, string enterer, char clientType, char action)
        {

            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditPhotos", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ClientId", clientId);
                        cmd.Parameters.AddWithValue("@PhotoData", photoData);
                        cmd.Parameters.AddWithValue("@PhotoExtension", photoExt);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@ClientType", clientType);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditPhotos method: " + ex);
                    }
                }
            }
            return result;
        }

        public DataTable GetSysData(char sysDataType)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSysData", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SysDataType", sysDataType);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSysData method: " + ex);
                    }

                }
            }
            return dt;
        }

        #region Static Data Modules DB Access Methods

        public DataTable GetStaticData(char staticDataType)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStaticData", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SD", staticDataType);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStaticData method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStaticData method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetClasses()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClasses", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClasses method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClasses method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetSysForms()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetSysForms", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSysForms method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetSysForms method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetClassStreams()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClassStreams", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClassStreams method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClassStreams method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int AddEditStaticData(StaticData staticData, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditStaticData", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        //input data
                        cmd.Parameters.AddWithValue("@SD", staticData.StaticDataType);
                        cmd.Parameters.AddWithValue("@SDCode", staticData.StaticDataCode);
                        cmd.Parameters.AddWithValue("@FCode", staticData.FormCode.Trim());
                        cmd.Parameters.AddWithValue("@SCode", staticData.StreamCode.Trim());
                        cmd.Parameters.AddWithValue("@SDDescription", staticData.Description);
                        cmd.Parameters.AddWithValue("@Deleted", staticData.Deleted);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditStaticData method: " + ex);
                        result = ex.Number;
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditStaticData method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public object GetStaticDataDetails(char sd, int sDid)
        {
            object result = null;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();
                    //input data
                    var param = new DynamicParameters();
                    param.Add("@SD", sd);
                    param.Add("@SDid", sDid);

                    switch (sd)
                    {
                        case 'H':
                            result = conn.Query<Houses>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'T':
                            result = conn.Query<Titles>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'P':
                            result = conn
                                .Query<Relationships>("sp_GetStaticDataDetails", param, null, true, 0,
                                    CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'R':
                            result = conn.Query<Religions>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'N':
                            result = conn
                                .Query<Nationalities>("sp_GetStaticDataDetails", param, null, true, 0,
                                    CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'S':
                            result = conn
                                .Query<ClassStreams>("sp_GetStaticDataDetails", param, null, true, 0,
                                    CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'C':
                            result = conn.Query<Classes>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'E':
                            result = conn.Query<Education>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'J':
                            result = conn.Query<Subjects>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                        case 'L':
                            result = conn.Query<Suppliers>("sp_GetStaticDataDetails", param, null, true, 0,
                                CommandType.StoredProcedure).SingleOrDefault();
                            break;
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStaticDataDetails method: " + ex);
                }

                return result;
            }
        }

        public bool StatiDataCodeExists(char sdType, string sdCode)
        {
            var codeExists = false;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStaticDataCode", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@SD", sdType);
                        cmd.Parameters.AddWithValue("@SDcode", sdCode.ToUpper());
                        cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var result = (int)cmd.Parameters["@result"].Value;

                        if (result > 0)
                        {
                            codeExists = true;
                        }

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at StatiDataCodeExists method: " + ex);
                    }

                }
            }
            return codeExists;
        }

        #endregion

        #region Search Module DB Access Methods

        public DataTable HelpGeneralSearch(char searchFlag)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpGeneralSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", searchFlag);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpGeneralSearch method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpGeneralSearch method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable HelpAccountSearch(char accountType)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpAccountSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpAccountSearch method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable HelpSchuleAccountSearch()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpSchuleAccountSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpSchuleAccountSearch method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpSchuleAccountSearch method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable HelpClientTypeProductSearch(char clientType)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpClientTypeProductSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ClientType", clientType);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpClientTypeProductSearch method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpClientTypeProductSearch method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable HelpFixedAssetSearch()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpFixedAssetSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpFixedAssetSearch method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at HelpFixedAssetSearch method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        #endregion

        public int EditSystemSettings(SystemSettings settings, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_EditSystemSettings", conn))
                {
                    try
                    {
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    //input data
                    cmd.Parameters.AddWithValue("@SysID", settings.SysID);
                    cmd.Parameters.AddWithValue("@PAddress", settings.PAddress.Trim());
                    cmd.Parameters.AddWithValue("@BoxAddress", settings.BoxAddress.Trim());
                    cmd.Parameters.AddWithValue("@Email1", settings.Email1.Trim());
                    cmd.Parameters.AddWithValue("@Email2", settings.Email2.Trim());
                    cmd.Parameters.AddWithValue("@Phone1", settings.Phone1.Trim());
                    cmd.Parameters.AddWithValue("@Phone2", settings.Phone2.Trim());
                    cmd.Parameters.AddWithValue("@TagLine", settings.TagLine.Trim());
                    cmd.Parameters.AddWithValue("@Website", settings.Website.Trim());
                    cmd.Parameters.AddWithValue("@Enterer", enterer);

                    result = cmd.ExecuteNonQuery();

                    conn.Close();
                }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at EditSystemSettings method: " + ex);
                    }
                }
            }
            return result;
        }

        public int EditOtherSettings(SystemSettings settings, string availablebal, string totalbal, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_EditOtherSettings", conn))
                {
                    try
                    {
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    //input data
                    cmd.Parameters.AddWithValue("@SysID", settings.SysID);
                    cmd.Parameters.AddWithValue("@PswdHistory", settings.PswdHistory);
                    cmd.Parameters.AddWithValue("@PswdAge", settings.PswdAge);
                    cmd.Parameters.AddWithValue("@PswdLength", settings.PswdLength);
                    cmd.Parameters.AddWithValue("@PswdComplexity", settings.PswdComplexity);
                    cmd.Parameters.AddWithValue("@LockSysPeriod", settings.LockSysPeriod);
                    cmd.Parameters.AddWithValue("@AvailableBal", availablebal);
                    cmd.Parameters.AddWithValue("@TotalBal", totalbal);
                    cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at EditOtherSettings method: " + ex);
                    }
                }
            }
            return result;
        }

        #region Parents Module DB Access Methods

        public int AddEditParents(Parents parent, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditParents", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ParentID", parent.ParentID);
                        cmd.Parameters.AddWithValue("@TitleCode", parent.TitleCode);
                        cmd.Parameters.AddWithValue("@SurName", parent.SurName);
                        cmd.Parameters.AddWithValue("@OtherNames", parent.OtherNames);
                        cmd.Parameters.AddWithValue("@NationalityCode", parent.NationalityCode);
                        cmd.Parameters.AddWithValue("@Occupation", parent.Occupation);
                        cmd.Parameters.AddWithValue("@EducationCode", parent.EducationCode);
                        cmd.Parameters.AddWithValue("@RAddress", parent.RAddress);
                        cmd.Parameters.AddWithValue("@RPhone1", parent.RPhone1);
                        cmd.Parameters.AddWithValue("@RPhone2", parent.RPhone2);
                        cmd.Parameters.AddWithValue("@REmailID1", parent.REmailID1);
                        cmd.Parameters.AddWithValue("@REmailID2", parent.REmailID2);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        //output parameter
                        cmd.Parameters.Add("@ParentID_Out", SqlDbType.Int);
                        cmd.Parameters["@ParentID_Out"].Direction = ParameterDirection.Output;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.VarChar);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            if (action == 'A')
                            {
                                result = Convert.ToInt32(cmd.Parameters["@ParentID_Out"].Value);
                            }                           
                        }

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditParents method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditParents method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public DataTable GetParentDetails(string parentId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_HelpGeneralSearch", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EntityID", parentId);
                        cmd.Parameters.AddWithValue("@Flag", 'P');

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetParentDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetParentDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        #endregion

        #region Students Module DB Access Methods

        public DataTable GetStudentDetails(string studentId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentPersonal", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StudentID", studentId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int AddEditStudents(Students student, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditStudents", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
                        cmd.Parameters.AddWithValue("@IndexNo", student.IndexNo);
                        cmd.Parameters.AddWithValue("@UNEBNo", student.UNEBNo);
                        cmd.Parameters.AddWithValue("@OtherID1", student.OtherID1);
                        cmd.Parameters.AddWithValue("@OtherID2", student.OtherID2);
                        cmd.Parameters.AddWithValue("@SurName", student.SurName);
                        cmd.Parameters.AddWithValue("@OtherNames", student.OtherNames);
                        cmd.Parameters.AddWithValue("@Dateofbirth", student.Dateofbirth);
                        cmd.Parameters.AddWithValue("@GenderID", student.GenderID);
                        cmd.Parameters.AddWithValue("@NationalityCode", student.NationalityCode);
                        cmd.Parameters.AddWithValue("@RegDate", student.RegDate);
                        cmd.Parameters.AddWithValue("@RAddress", student.RAddress);
                        cmd.Parameters.AddWithValue("@ReligionCode", student.ReligionCode);
                        cmd.Parameters.AddWithValue("@ClassCode", student.ClassCode);
                        cmd.Parameters.AddWithValue("@HouseCode", student.HouseCode);
                        cmd.Parameters.AddWithValue("@FormerSchool", student.FormerSchool);
                        cmd.Parameters.AddWithValue("@SchSectionID", student.SchSectionID);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        cmd.Parameters.Add("@StudentID_Out", SqlDbType.Int);
                        cmd.Parameters["@StudentID_Out"].Direction = ParameterDirection.Output;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.VarChar);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            if (action == 'A')
                            {
                                result = Convert.ToInt32(cmd.Parameters["@StudentID_Out"].Value);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditStudents method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditStudents method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return result;
        }

        public DataTable GetStudentParents(string studentId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentParents", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentID", studentId);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentParents method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentParents method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int AddEditStudentParents(StudentParent studentParents, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditStudentParents", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@StudentID", studentParents.StudentID);
                        cmd.Parameters.AddWithValue("@ParentID", studentParents.ParentID);
                        cmd.Parameters.AddWithValue("@RshipCode", studentParents.RshipCode);
                        cmd.Parameters.AddWithValue("@LiveswithStudent", studentParents.LiveswithStudent);
                        cmd.Parameters.AddWithValue("@PaysFees", studentParents.PaysFees);
                        cmd.Parameters.AddWithValue("@MoreInfo", studentParents.MoreInfo);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);


                        result = cmd.ExecuteNonQuery();
                        
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at AddEditStudentParents method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at AddEditStudentParents method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return result;
        }

        #endregion

        public int AddEditStudentSpecialFees(StudentSpecialFees specialFees, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditStudentSpecialFees", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@SFID", specialFees.SFID);
                        cmd.Parameters.AddWithValue("@StudentID", specialFees.StudentID);
                        cmd.Parameters.AddWithValue("@ProductCode", specialFees.ProductCode);
                        cmd.Parameters.AddWithValue("@FeeCode", specialFees.FeeCode);
                        cmd.Parameters.AddWithValue("@TermID", specialFees.TermID);
                        cmd.Parameters.AddWithValue("@PercentageOrAmt", specialFees.PercentageOrAmount);
                        cmd.Parameters.AddWithValue("@Value", specialFees.Value);
                        cmd.Parameters.AddWithValue("@Remarks", specialFees.Remarks);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);


                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditStudentSpecialFees method: " + ex);
                    }

                }
            }
            return result;
        }

        public DataTable GetClients(char clientType)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClients", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClientType", clientType);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch(Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClients method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetStudentSpecialFees(string studentId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentSpecialFees", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentID", studentId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentSpecialFees method: " + ex);
                    }
                }
            }
            return dt;
        }

        public object GetClientDetails(string clientId, char clientType, char action)
        {
            object clientDetails = null;

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("@ClientId", clientId);
                    param.Add("@ClientType", clientType);
                    param.Add("@Action", action);
                    clientDetails = con.Query<object>("sp_GetClientDetails", param, null, true, 0,
                        CommandType.StoredProcedure).SingleOrDefault();
                    con.Close();
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClientDetails method: " + ex);
                }
                return clientDetails;
            }
        }

        public ClientPhotos GetClientPhotoDetails(string clientId, char clientType)
        {
            ClientPhotos clientPhotoDetails = null;

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("@ClientId", clientId);
                    param.Add("@ClientType", clientType);
                    clientPhotoDetails = con.Query<ClientPhotos>("sp_GetClientPhotoDetails", param, null, true, 0,
                        CommandType.StoredProcedure).SingleOrDefault();
                    con.Close();
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClientPhotoDetails method: " + ex);
                }
            }
            return clientPhotoDetails;
        }

        #region Schule Products DB Access Methods

        public DataTable GetClientTypes()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClientTypes", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClientTypes method: " + ex);
                    }

                }
            }
            return dt;
        }

        public DataTable GetProductTypes()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductTypes", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductTypes method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductTypes method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetProductsByClientType(char clienttype)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductsByClientType", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClientTypeCode", clienttype);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductsByClientType method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetProductTypesByClientType(char clienttype)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductTypesByClientType", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClientTypeCode", clienttype);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetProductTypesByClientType method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetProductTypesByClientType method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetProducts()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProducts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProducts method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetProductItems(string productCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductItems", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductItems method: " + ex);
                    }
                }
            }
            return dt;
        }

        public int AddEditProducts(Products product, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditProducts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@ClientTypeCode", product.ClientTypeCode);
                        cmd.Parameters.AddWithValue("@ProductTypeCode", product.ProductTypeCode);
                        cmd.Parameters.AddWithValue("@AllowCredits", product.AllowCredits);
                        cmd.Parameters.AddWithValue("@CanGoInCredit", product.CanGoInCredit);
                        cmd.Parameters.AddWithValue("@AllowDebits", product.AllowDebits);
                        cmd.Parameters.AddWithValue("@CanGoInDebit", product.CanGoInDebit);
                        cmd.Parameters.AddWithValue("@ControlAccountGl", product.ControlAccountGL);
                        cmd.Parameters.AddWithValue("@ContraAccountGl", product.ContraAccountGL);
                        cmd.Parameters.AddWithValue("@PAndLAccountGl", product.PAndLAccountGL);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditProducts method: " + ex);
                        result = ex.Number;
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditProducts method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public int AddEditProductItems(ProductItems items, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditProductItems", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ItemCode", items.ItemCode);
                        cmd.Parameters.AddWithValue("@Description", items.Description);
                        cmd.Parameters.AddWithValue("@ProductCode", items.ProductCode);
                        cmd.Parameters.AddWithValue("@PAndLAccountGL", items.PAndLAccountGL);
                        cmd.Parameters.AddWithValue("@ContraAccountGL", items.ContraAccountGL);
                        cmd.Parameters.AddWithValue("@ApplyTo", items.ApplyTo);
                        cmd.Parameters.AddWithValue("@Deleted", items.Deleted);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditProductItems method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditProductItems method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return result;
        }


        #endregion

        #region Academics Module DB Access Methods

        public DataTable GetAcademicPeriods(int year)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAcademicPeriods", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AcademicYear", year);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAcademicPeriods method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetAcademicTerms()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAcademicTerms", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAcademicTerms method: " + ex);
                    }
                }
            }
            return dt;
        }

        public int AddEditAcademicPeriods(DataTable dt, int year, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditAcademicPeriods", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AcademicYear", year);
                        cmd.Parameters.AddWithValue("@AcademicPeriods", dt);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditAcademicPeriods method: " + ex);
                    }

                }
            }
            return result;
        }

        public DataTable GetStudentRegDetails(int academicId, char action)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentsRegDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicID", academicId);
                        cmd.Parameters.AddWithValue("@Action", action);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentRegDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentRegDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int UpdateStudentRegistration(int academicId, string studentId, string classCode, char schSectionId, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateStudentRegistration", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AcademicID", academicId);
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@ClassCode", classCode);
                        cmd.Parameters.AddWithValue("@SchSectionID", schSectionId);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at UpdateStudentRegistration method: " + ex.Message);
                    }
                }
            }
            return result;
        }

        public int RegisterUnregisterStudents(int academicId, string studentId, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_RegUnRegStudents", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AcademicID", academicId);
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at RegisterUnregisterStudents method: " + ex.Message);
                    }
                }
            }
            return result;
        }

        #endregion

        #region SchuleTransactions DB Access Methods
        public DataTable GetStudentsToBill(int academicId, string productCode, string feeCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentsToBill", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicID", academicId);
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);
                        cmd.Parameters.AddWithValue("@FeeCode", feeCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentsToBill method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetTranCategories()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetTranCategories", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTranCategories method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetTransactionsView(string tranCat, string status, string postedby = "", DateTime? opDate = null)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetTransactionView", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TranCat", tranCat);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@OPDate", opDate);
                        cmd.Parameters.AddWithValue("@PostedBy", postedby);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                        
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTransactionsView method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetProductContraGl(string productCode, string feeCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductContraGL", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);
                        cmd.Parameters.AddWithValue("@FeeCode", feeCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductContraGl method: " + ex);
                    }
                }
            }
            return dt;
        }

        public int AddEditTransactions(DateTime tranDate, char tranType, char tranCat,  DataTable trans, string enterer, char action, string glSubHead = "", string feeCode = "", bool isSysTran = false, bool isFaTrans = false, string faProdCode = "", int tranId = 0)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditTransactions", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data 
                        cmd.Parameters.AddWithValue("@TranDate", tranDate);
                        cmd.Parameters.AddWithValue("@TranType", tranType);
                        cmd.Parameters.AddWithValue("@TranCat", tranCat);
                        cmd.Parameters.AddWithValue("@NewTran", trans);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@GLSubHead", glSubHead);
                        cmd.Parameters.AddWithValue("@FeeCode", feeCode);
                        cmd.Parameters.AddWithValue("@IsSysTran", isSysTran);                    
                        cmd.Parameters.AddWithValue("@TranID", tranId);
                        
                        //output parameter
                        cmd.Parameters.Add("@TranID_out", SqlDbType.Int);
                        cmd.Parameters["@TranID_out"].Direction = ParameterDirection.Output;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            if (action == 'A')
                            {
                                result = Convert.ToInt32(cmd.Parameters["@TranID_out"].Value);
                            }                           
                        }
                        else
                        {
                            result = (int) returnParameter.Value;
                        }

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditTransactions method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public int SuperviseTransactions(DateTime tranDate, decimal tranId, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_SuperviseTransactions", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data 
                        cmd.Parameters.AddWithValue("@TranDate", tranDate);
                        cmd.Parameters.AddWithValue("@TranID", tranId);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            result = (int)returnParameter.Value;
                        }

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at SuperviseTransactions method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public DataTable GetTransactionDetails(int tranId, DateTime? opDate = null)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetTranDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TranID", tranId);
                        cmd.Parameters.AddWithValue("@OPDate", opDate);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTransactionDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public GlAccounts GetCashierGl(string username)
        {
            GlAccounts cashierGl = null;

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ToString()))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("@Username", username);
                    cashierGl = con.Query<GlAccounts>("sp_GetCashierGL", param, null, true, 0,
                        CommandType.StoredProcedure).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetCashierGl method: " + ex);
                }
                finally
                {
                    con.Close();
                }
            }
            return cashierGl;
        }

        public DataTable GetCashRegister(DateTime fromDate, DateTime toDate, string partTranType, string operatorId, string cashiergl)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("rpt_CashRegister", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);
                        cmd.Parameters.AddWithValue("@PartTranType", partTranType);
                        cmd.Parameters.AddWithValue("@OperatorID", operatorId);
                        cmd.Parameters.AddWithValue("@CashierGL", cashiergl);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetCashRegister method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetCashRegister method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetStudentOsBills(string accountId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetStudentOSBills", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AccountID", accountId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentOsBills method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetAccountDetails(string accountId, char accountType, bool isCashierGl = false)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAccountDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                        cmd.Parameters.AddWithValue("@IsCashierGL", isCashierGl);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAccountDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public decimal GetAccountBalance(string accountId, char accountType, bool isVerified, bool isCashierGl)
        {
            decimal clearBalance = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT dbo.fn_ClearBalance(@AccountID, @Accounttype, @IsVerified, @IsCashierGL)", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@Accounttype", accountType);
                        cmd.Parameters.AddWithValue("@IsVerified", isVerified);
                        cmd.Parameters.AddWithValue("@IsCashierGL", isCashierGl);
                        clearBalance = (decimal) cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAccountBalance method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAccountBalance method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return clearBalance;
        }

        public decimal GetUnsupervisedDrcr(string accountId, char accountType, char drcr)
        {
            decimal amount = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT dbo.fn_UnSupervisedDRCR(@AccountID, @Accounttype, @DRCR)", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@Accounttype", accountType);
                        cmd.Parameters.AddWithValue("@DRCR", drcr);
                        amount = (decimal) cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUnsupervisedrcr method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetUnsupervisedrcr method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return amount;
        }

        public decimal GetTotalCreditsDebits(string accountId, char partTranType)
        {

            decimal amount = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT dbo.fn_TotalCreditsDebits(@AccountID, @PartTranType)", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@PartTranType", partTranType);
                        amount = (decimal)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTotalCreditsDebits method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTotalCreditsDebits method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return amount;
        }
        #endregion

        #region Schule Client Accounts DB Access Methods

        public int VerifyAccount(string accountId, char accountType, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_VerifyAccount", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            result = (int)returnParameter.Value;
                        }

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at VerifyAccount method: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at VerifyAccount method: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public DataTable GetUnVerifiedAccounts(string enterer, char accountType, string productCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetUnVerifiedAccounts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetUnVerifiedAccounts method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetUnVerifiedAccounts method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetClientsWithoutAccounts(char clientType, string productCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClientsWithoutAccounts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClientTypeID", clientType);
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetClientsWithoutAccounts method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath,
                            "Db Transaction Error at GetClientsWithoutAccounts method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return dt;
        }

        public DataTable GetProductDetails(string productCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetProductDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetProductDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int GenerateAccount(string clientId, string product, DateTime accountOpenDate, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GenerateAccount", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ClientID", clientId);
                        cmd.Parameters.AddWithValue("@ProductCode", product);
                        cmd.Parameters.AddWithValue("@OpenDate", accountOpenDate);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GenerateAccount method: " + ex.Message);
                    }
                }
            }
            return result;
        }

        public int CloseAccount(string account, string closeReason, char accountType, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_CloseAccount", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AccountID", account);
                        cmd.Parameters.AddWithValue("@CloseReason", closeReason);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at CloseAccount method: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at CloseAccount method: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        #endregion

        #region Schule Fees DB Access Methods

        public DataTable GetTermFees(string productCode, string feeCode, string schSectionId, int year, int termId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetTermFees", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ProductCode", productCode);
                        cmd.Parameters.AddWithValue("@FeeCode", feeCode);
                        cmd.Parameters.AddWithValue("@SchSectionID", schSectionId);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@TermID", termId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTermFees method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetTermFees method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public int AddEditTermFees(TermFees fees, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditTermFees", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@ProductCode", fees.ProductCode);
                        cmd.Parameters.AddWithValue("@FeeCode", fees.FeeCode);
                        cmd.Parameters.AddWithValue("@SchSectionID", fees.SchSectionID);
                        cmd.Parameters.AddWithValue("@YearID", fees.Year);
                        cmd.Parameters.AddWithValue("@TermID", fees.TermID);
                        cmd.Parameters.AddWithValue("@FormCode", fees.FormCode);
                        cmd.Parameters.AddWithValue("@Amount", fees.Amount);
                        cmd.Parameters.AddWithValue("@AllForms", fees.AllForms);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditTermFees method: " + ex);
                        result = ex.Number;
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditTermFees method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }
            return result;
        }

        #endregion

        #region General Ledgers Module DB Access Methods

        public DataTable GetGlTypes()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetGlTypes", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlTypes method: " + ex);
                    }

                }
            }
            return dt;
        }

        public DataTable GetGlSubTypes()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetGlSubTypes", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlSubTypes method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlSubTypes method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetGlSubTypesByGlType(char gltype)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetGlSubTypesByGlType", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GlTypeCode", gltype);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlSubTypesByGlType method: " + ex);
                    }
                }
            }
            return dt;
        }

        public DataTable GetGlParameters()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetGLParameters", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlParameters method: " + ex);
                    }

                }
            }
            return dt;
        }

        public int AddEditGl(GlAccounts account, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditGL", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@AccountID", account.AccountID);
                        cmd.Parameters.AddWithValue("@AccountName", account.AccountName);
                        cmd.Parameters.AddWithValue("@AccountType", account.AccountType);
                        cmd.Parameters.AddWithValue("@AccountSubType", account.AccountSubType);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        //output parameter
                        cmd.Parameters.Add("@NewGLAccountID", SqlDbType.VarChar);
                        cmd.Parameters["@NewGLAccountID"].Direction = ParameterDirection.Output;

                        if (action == 'E')
                        {
                            result = cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            var returnParameter = cmd.Parameters.Add("@NewGLAccountID", SqlDbType.VarChar);
                            returnParameter.Direction = ParameterDirection.ReturnValue;

                            cmd.ExecuteNonQuery();
                            result = (int)returnParameter.Value;
                        }
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditGl method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditGl method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public int AddEditGlSubTypes(GlSubTypes glsubtypes, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditGLSubTypes", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@GLSubTypeCode", glsubtypes.GLSubTypeCode);
                        cmd.Parameters.AddWithValue("@GLSubType", glsubtypes.GLSubType);
                        cmd.Parameters.AddWithValue("@GLTypeCode", glsubtypes.GLTypeCode);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditGlSubTypes method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditGlSubTypes method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public int EditGlParameters(GlParameters parameters, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_EditGLParameters", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //input data
                        cmd.Parameters.AddWithValue("@SerialID", parameters.SerialID);
                        cmd.Parameters.AddWithValue("@AccountID", parameters.AccountID);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at EditGlParameters method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at EditGlParameters method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        #endregion

        #region Fixed Assets Module DB Access Methods

        public DataTable GetFixedAssetProductDetails(string productCode)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetFixedAssetProductDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetFixedAssetProductDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetFixedAssetProductDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetFixedAssetDetails(string assetId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetFixedAssetDetails", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssetID", assetId);

                        var da = new SqlDataAdapter {SelectCommand = cmd};

                        da.Fill(dt);

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAssetDetails method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetAssetDetails method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public string AddEditFixedAssets(FixedAssets asset, char action, string enterer)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditFixedAssets", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        //input data
                        cmd.Parameters.AddWithValue("@AssetID", asset.AssetID);
                        cmd.Parameters.AddWithValue("@Description", asset.Description);
                        cmd.Parameters.AddWithValue("@ProductCode", asset.ProductCode);
                        cmd.Parameters.AddWithValue("@SupplierID", asset.SupplierID);
                        cmd.Parameters.AddWithValue("@SerialNo", asset.SerialNo);
                        cmd.Parameters.AddWithValue("@TagNo", asset.TagNo);
                        cmd.Parameters.AddWithValue("@BrandName", asset.BrandName);
                        cmd.Parameters.AddWithValue("@Location", asset.Location);
                        cmd.Parameters.AddWithValue("@DepMthd", asset.DepMthd);
                        cmd.Parameters.AddWithValue("@DepRate", asset.DepRate);
                        cmd.Parameters.AddWithValue("@CostPrice", asset.CostPrice);
                        cmd.Parameters.AddWithValue("@Terms", asset.Terms);
                        cmd.Parameters.AddWithValue("@PurchasedOn", asset.PurchasedOn);
                        cmd.Parameters.AddWithValue("@DepFrom", asset.DepFrom);
                        cmd.Parameters.AddWithValue("@DepEnd", asset.DepEnd);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        //output parameter
                        cmd.Parameters.Add("@NewAssetID", SqlDbType.VarChar, 15);
                        cmd.Parameters["@NewAssetID"].Direction = ParameterDirection.Output;

                        var result = cmd.ExecuteNonQuery();

                        if (result > 0 && action == 'A')
                        {
                            return cmd.Parameters["@NewAssetID"].Value.ToString();
                        }
                        else if(result > 0)
                        {
                            return "update successful";
                        }                   
                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditFixedAssets method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditFixedAssets method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return null;
        }

        public int AddEditFixedAssetsProducts(FixedAssetsProducts product, char action, string enterer)
        {
            var result = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEditFixedAssetsProducts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        //input data
                        cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@AssetIDPrefix", product.AssetIDPrefix);
                        cmd.Parameters.AddWithValue("@AllowCredits", product.AllowCredits);
                        cmd.Parameters.AddWithValue("@CanGoInCredit", product.CanGoInCredit);
                        cmd.Parameters.AddWithValue("@AllowDebits", product.AllowDebits);
                        cmd.Parameters.AddWithValue("@CanGoInDebit", product.CanGoInDebit);
                        cmd.Parameters.AddWithValue("@ControlAccountGL", product.ControlAccountGL);
                        cmd.Parameters.AddWithValue("@AccumDepGL", product.AccumDepGL);
                        cmd.Parameters.AddWithValue("@DepExpenseGL", product.DepExpenseGL);
                        cmd.Parameters.AddWithValue("@SaleoffLossGL", product.SaleoffLossGL);
                        cmd.Parameters.AddWithValue("@SaleoffProfitGL", product.SaleoffProfitGL);
                        cmd.Parameters.AddWithValue("@Deleted", product.Deleted);
                        cmd.Parameters.AddWithValue("@Enterer", enterer);
                        cmd.Parameters.AddWithValue("@Action", action);

                        result = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditFixedAssetsProducts method: " + ex);
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddEditFixedAssetsProducts method: " + ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        #endregion


        public bool ClientPhotoExists(string clientId, char clientType)
        {
            var result = false;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd;
                    if (clientType == 'P')
                    {
                        cmd = new SqlCommand("Select * from t_ParentPhotos where ParentID=@clientId", conn);
                        cmd.Parameters.AddWithValue("@clientId", clientId.Trim());
                    }
                    else
                    {
                        cmd = new SqlCommand("Select * from t_StudentPhotos where StudentID=@clientId", conn);
                        cmd.Parameters.AddWithValue("@clientId", clientId.Trim());
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        result = reader.Read();
                    conn.Close();
                }
            }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at ClientPhotoExists method: " + ex);
                }
            }
            return result;
        }

        public DataTable GetStudentFeesProductDetails()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT P.ProductCode, P.ProductName, I.ItemCode, I.Description FROM t_Products P, t_ProductItems I WHERE P.ProductCode = I.ProductCode AND ProductTypeCode = 'F'", conn);
                    var da = new SqlDataAdapter { SelectCommand = cmd };
                    da.Fill(dt);

                    conn.Close();
                }
                catch (Exception ex)
                {
                    _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetStudentFeesProductDetails method: " + ex);
                }
            }
            return dt;
        }

        public DataTable GetClientAccounts(char clientType, string productId)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClientAccounts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ClientType", clientType);
                        cmd.Parameters.AddWithValue("@Product", productId);

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetClientAccounts method: " + ex);
                    }

                }
            }
            return dt;
        }



        public DataTable GetGlAccounts()
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetGlAccounts", conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        var da = new SqlDataAdapter { SelectCommand = cmd };

                        da.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at GetGlAccounts method: " + ex);
                    }

                }
            }
            return dt;
        }

        public void AddAuditData(Audit audit)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Con].ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddAuditData", conn))
                {
                    try
                    {
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    //input data
                    cmd.Parameters.AddWithValue("@UserName", audit.UserName);
                    cmd.Parameters.AddWithValue("@SessionId", audit.SessionId);
                    cmd.Parameters.AddWithValue("@IpAddress", audit.IpAddress);
                    cmd.Parameters.AddWithValue("@PageAccessed", audit.PageAccessed);
                    cmd.Parameters.AddWithValue("@AccessedTime", audit.AccessedTime);
                    cmd.Parameters.AddWithValue("@LoggedOutAt", audit.LoggedOutAt);
                    cmd.Parameters.AddWithValue("@LoginStatus", audit.LoginStatus);
                    cmd.Parameters.AddWithValue("@ControllerName", audit.ControllerName);
                    cmd.Parameters.AddWithValue("@ActionName", audit.ActionName);
                    cmd.Parameters.AddWithValue("@UrlReferrer", audit.UrlReferrer);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                    catch (Exception ex)
                    {
                        _clf.CreateErrorLog(ErrorLogPath, "Db Transaction Error at AddAuditData method: " + ex);
                    }
                }
            }
        }
    }

}