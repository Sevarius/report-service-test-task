using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EnsureThat;
using Npgsql;
using ReportService.Application.Repositories;
using ReportService.Models.PocoModels;

namespace ReportService.Infrastructure.Repositories
{
    internal sealed class EmployeeRepository : IEmployeeRepository, IDisposable
    {
        public EmployeeRepository(string connectionString)
        {
            EnsureArg.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
            
            this._connectionString = connectionString;
        }
        
        private readonly string _connectionString;

        private IDbConnection _connection;

        private IDbConnection Connection
        {
            get 
            {
                if (this._connection != null)
                {
                    return this._connection;
                }

                this._connection = new NpgsqlConnection(this._connectionString);
                this._connection.Open();

                return this._connection;
            }
        }

        public async Task<IReadOnlyCollection<DepartmentPocoModel>> ListActiveDepartmentsAsync()
        {
            var departments = await this.Connection
                .QueryAsync<DepartmentPocoModel>(ListDepartmentsSql)
                .ConfigureAwait(false);

            return departments.ToArray();
        }
        
        public async Task<IReadOnlyCollection<EmployeePocoModel>> ListEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            var employees = await this.Connection
                .QueryAsync<EmployeePocoModel>(ListEmployeesSql, new { departmentId })
                .ConfigureAwait(false);

            return employees.ToArray();
        }

        public void Dispose() => _connection?.Dispose();

        #region Sql Queries
        
        private const string ListDepartmentsSql = "select d.name from deps d where d.active = true";
        private const string ListEmployeesSql = "SELECT e.name, e.inn from emps e where e.departmentid = @departmentId";
        
        #endregion
    }
}
