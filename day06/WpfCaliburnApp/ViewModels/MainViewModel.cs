using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCaliburnApp.Models;

namespace WpfCaliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        /*string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        //Caliburn 규칙 : 클래스명을 동일하게 ex) CanSayHello, SayHello
        public bool CanSayHello
        {
            get { return !string.IsNullOrEmpty(Name); }
        }
        public void SayHello()
        {
            MessageBox.Show($"Hello~{Name}");
        }*/

        //private EmployeesModel employee;   //VS 제안하는 이름 사용 권장

        //List -> BindableCollection Type으로 변경 (MVVM에서 List가 문제가 생김)
        //ObservableCollection을 상속받은 BindableCollection으로 바인딩된 컨트롤과 Model을 안정적으로 동기화할 수 있다.
        private BindableCollection<EmployeesModel> listEmployees;
        public BindableCollection<EmployeesModel> ListEmployees
        {
            get { return listEmployees; }
            set
            {
                listEmployees = value;
                NotifyOfPropertyChange(() => ListEmployees);
            }
        }

        //서버 탐색기 고급탭에서 검색해서 적용
        string connString = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

        public MainViewModel()
        {
            GetEmployees();
        }

        public void GetEmployees()
        {
            //using 사용할 경우 Open 이후에 Close 생략가능
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string strQuery = "SELECT * FROM TblEmployees";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListEmployees = new BindableCollection<EmployeesModel>();

                while (reader.Read())
                {
                    var temp = new EmployeesModel
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Destination = reader["Destination"].ToString(),
                    };
                    ListEmployees.Add(temp);
                }
            }
        }

        int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanDelEmployee);
            }
        }

        string empName;
        public string EmpName
        {
            get { return empName; }
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        decimal salary;
        public decimal Salary
        {
            get { return salary; }
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => Salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string deptName;
        public string DeptName
        {
            get { return deptName; }
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string destination;
        public string Destination
        {
            get { return destination; }
            set
            {
                destination = value;
                NotifyOfPropertyChange(() => Destination);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        private EmployeesModel selectedEmployee;

        public EmployeesModel SelectedEmployee
        {
            get { return selectedEmployee; }
            set
            {
                selectedEmployee = value;
                if (value != null)
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Destination = value.Destination;
                }
                NotifyOfPropertyChange(() => SelectedEmployee);
            }
        }

        public void NewEmployee()
        {
            Id = 0;
            EmpName = string.Empty;
            Salary = 0;
            DeptName = Destination = string.Empty;

        }

        public bool CanSaveEmployee
        {
            get
            {
                return !string.IsNullOrEmpty(EmpName) &&
                        !string.IsNullOrEmpty(DeptName) &&
                        !string.IsNullOrEmpty(Destination) &&
                        Salary != 0;

            }
        }

        public void SaveEmployee()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Id == 0) //insert
                {
                    cmd.CommandText = @"INSERT INTO TblEmployees
                                               (EmpName
                                               ,Salary
                                               ,DeptName
                                               ,Destination)
                                         VALUES
                                               (@EmpName
                                               ,@Salary
                                               ,@DeptName
                                               ,@Destination)";
                }
                else //update
                {
                    cmd.CommandText = @"UPDATE TblEmployees
                                          SET EmpName = @EmpName
                                             ,Salary = @Salary
                                             ,DeptName = @DeptName
                                             ,Destination = @Destination
                                        WHERE Id = @Id";
                }

                SqlParameter parmEmpName = new SqlParameter("@EmpName", EmpName);
                SqlParameter parmSalary = new SqlParameter("@Salary", Salary);
                SqlParameter parmDeptName = new SqlParameter("@DeptName", DeptName);
                SqlParameter parmDestination = new SqlParameter("@Destination", Destination);
                cmd.Parameters.Add(parmEmpName);
                cmd.Parameters.Add(parmSalary);
                cmd.Parameters.Add(parmDeptName);
                cmd.Parameters.Add(parmDestination);

                if (Id != 0) //update일때는 Id값도 파라미터로 추가해야함
                {
                    SqlParameter parmId = new SqlParameter("@Id", Id);
                    cmd.Parameters.Add(parmId);
                }

                cmd.ExecuteNonQuery();
            }   //End of using (SqlConnection ...

            NewEmployee();  // 입력창 전부 초기화
            GetEmployees(); // 데이터 다시 조회
        }

        public bool CanDelEmployee
        {
            get { return (Id != 0); }
        }

        public void DelEmployee()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string strQuery = "DELETE FROM TblEmployees WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlParameter parmId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(parmId);

                cmd.ExecuteNonQuery();
            }

            NewEmployee();  // 입력창 전부 초기화
            GetEmployees(); // 데이터 다시 조회
        }
    }
}
