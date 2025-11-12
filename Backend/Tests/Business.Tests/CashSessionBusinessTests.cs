using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Business.Implementations;
using Entity.Context;
using Entity.Model;
using Entity.Dto;

namespace Business.Tests
{
    public class CashSessionBusinessTests
    {
        private ApplicationDbContext CreateContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task OpenSessionAsync_CreatesSessionAndOpeningMovement()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);
            var business = new CashSessionBusiness(
                cashSessionData: null!, // not used in these tests; base class won't be invoked
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            var dto = await business.OpenSessionAsync(100m);

            Assert.NotNull(dto);
            Assert.Equal(100m, dto.OpeningAmount);

            var sessions = context.cashSessions.ToList();
            Assert.Single(sessions);
            var session = sessions.First();
            Assert.Equal(100m, session.OpeningAmount);
            Assert.Null(session.ClosedAt);

            var movements = context.cashMovements.ToList();
            Assert.Single(movements);
            var mv = movements.First();
            Assert.Equal("Opening", mv.Type);
            Assert.Equal(100m, mv.Amount);
            Assert.Equal(session.Id, mv.CashSessionId);
        }

        [Fact]
        public async Task OpenSessionAsync_NegativeAmount_ThrowsArgumentException()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);
            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() => business.OpenSessionAsync(-1m));
        }

        [Fact]
        public async Task OpenSessionAsync_WhenOpenSessionExists_ThrowsInvalidOperationException()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);

            // Seed an open session
            var existing = new CashSession
            {
                OpenedAt = DateTime.UtcNow.AddHours(-1),
                ClosedAt = null,
                OpeningAmount = 50m,
                ClosingAmount = 0m,
                Active = true,
                CreateAt = DateTime.UtcNow.AddHours(-1),
                UpdateAt = DateTime.UtcNow.AddHours(-1)
            };
            context.cashSessions.Add(existing);
            await context.SaveChangesAsync();

            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            await Assert.ThrowsAsync<InvalidOperationException>(() => business.OpenSessionAsync(10m));
        }

        [Fact]
        public async Task CloseSessionAsync_CalculatesDifferenceAndCreatesClosingMovement()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);

            // Seed session with movements
            var session = new CashSession
            {
                OpenedAt = DateTime.UtcNow.AddHours(-2),
                ClosedAt = null,
                OpeningAmount = 100m,
                ClosingAmount = 0m,
                Active = true,
                CreateAt = DateTime.UtcNow.AddHours(-2),
                UpdateAt = DateTime.UtcNow.AddHours(-2)
            };
            context.cashSessions.Add(session);
            await context.SaveChangesAsync();

            // Entries: Sale 30, Deposit 20 => entries = 50
            context.cashMovements.AddRange(new[]
            {
                new CashMovement { CashSessionId = session.Id, Type = "Sale", Amount = 30m, Reason = "Venta", At = DateTime.UtcNow.AddHours(-1) },
                new CashMovement { CashSessionId = session.Id, Type = "Deposit", Amount = 20m, Reason = "Deposito", At = DateTime.UtcNow.AddHours(-1) },
                // Exits: Expense 10 => exits = 10
                new CashMovement { CashSessionId = session.Id, Type = "Expense", Amount = 10m, Reason = "Gasto", At = DateTime.UtcNow.AddHours(-1) }
            });
            await context.SaveChangesAsync();

            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            // expected = 100 + 50 - 10 = 140
            var difference = await business.CloseSessionAsync(session.Id, 150m); // closingAmount 150 => difference 10

            Assert.Equal(10m, difference);

            var updated = await context.cashSessions.FindAsync(session.Id);
            Assert.NotNull(updated);
            Assert.NotNull(updated!.ClosedAt);
            Assert.Equal(150m, updated.ClosingAmount);

            var closingMovements = context.cashMovements.Where(m => m.Type == "Closing" && m.CashSessionId == session.Id).ToList();
            Assert.Single(closingMovements);
            Assert.Equal(150m, closingMovements[0].Amount);
        }

        [Fact]
        public async Task CloseSessionAsync_NonExistingSession_ThrowsKeyNotFoundException()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);
            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => business.CloseSessionAsync(9999, 100m));
        }

        [Fact]
        public async Task GetOpenSessionAsync_ReturnsOpenSessionDtoOrNull()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);

            // No sessions -> null
            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            var none = await business.GetOpenSessionAsync();
            Assert.Null(none);

            // Seed open session
            var session = new CashSession
            {
                OpenedAt = DateTime.UtcNow,
                ClosedAt = null,
                OpeningAmount = 25m,
                ClosingAmount = 0m,
                Active = true,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
            context.cashSessions.Add(session);
            await context.SaveChangesAsync();

            var found = await business.GetOpenSessionAsync();
            Assert.NotNull(found);
            Assert.Equal(session.Id, found!.Id);
        }

        [Fact]
        public async Task GetByDateRangeAsync_ReturnsSessionsWithinRange()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);

            var now = DateTime.UtcNow;
            var s1 = new CashSession { OpenedAt = now.AddDays(-10), ClosedAt = now.AddDays(-9), OpeningAmount = 10m, ClosingAmount = 10m, Active = true, CreateAt = now.AddDays(-10), UpdateAt = now.AddDays(-10) };
            var s2 = new CashSession { OpenedAt = now.AddDays(-5), ClosedAt = now.AddDays(-4), OpeningAmount = 20m, ClosingAmount = 20m, Active = true, CreateAt = now.AddDays(-5), UpdateAt = now.AddDays(-5) };
            var s3 = new CashSession { OpenedAt = now.AddDays(-1), ClosedAt = null, OpeningAmount = 30m, ClosingAmount = 0m, Active = true, CreateAt = now.AddDays(-1), UpdateAt = now.AddDays(-1) };

            context.cashSessions.AddRange(s1, s2, s3);
            await context.SaveChangesAsync();

            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            var from = now.AddDays(-6);
            var to = now;
            var results = (await business.GetByDateRangeAsync(from, to)).ToList();

            Assert.Equal(2, results.Count); // s2 and s3
            Assert.Contains(results, r => r.Id == s2.Id);
            Assert.Contains(results, r => r.Id == s3.Id);
        }

        [Fact]
        public async Task GetSessionBalanceAsync_ReturnsCorrectBalanceAndMovements()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using var context = CreateContext(connection);

            var session = new CashSession
            {
                OpenedAt = DateTime.UtcNow.AddHours(-3),
                ClosedAt = null,
                OpeningAmount = 200m,
                ClosingAmount = 0m,
                Active = true,
                CreateAt = DateTime.UtcNow.AddHours(-3),
                UpdateAt = DateTime.UtcNow.AddHours(-3)
            };
            context.cashSessions.Add(session);
            await context.SaveChangesAsync();

            // Movements: Sale 50, Deposit 20, Expense 10 => expected = 200 + (50+20) - 10 = 260
            context.cashMovements.AddRange(new[]
            {
                new CashMovement { CashSessionId = session.Id, Type = "Sale", Amount = 50m, Reason = "Venta", At = DateTime.UtcNow.AddHours(-2) },
                new CashMovement { CashSessionId = session.Id, Type = "Deposit", Amount = 20m, Reason = "Deposito", At = DateTime.UtcNow.AddHours(-2) },
                new CashMovement { CashSessionId = session.Id, Type = "Expense", Amount = 10m, Reason = "Gasto", At = DateTime.UtcNow.AddHours(-1) }
            });
            await context.SaveChangesAsync();

            var business = new CashSessionBusiness(
                cashSessionData: null!,
                context: context,
                logger: NullLogger<BaseBusiness<CashSession, CashSessionDto>>.Instance);

            var balance = await business.GetSessionBalanceAsync(session.Id);

            Assert.Equal(session.Id, balance.SessionId);
            Assert.Equal(200m, balance.OpeningAmount);
            Assert.Equal(260m, balance.ExpectedAmount);
            Assert.Equal(260m, balance.ActualAmount); // session open -> actual == expected
            Assert.Equal(0m, balance.Difference);
            Assert.Equal(3, balance.Movements.Count);
            Assert.Equal("Sale", balance.Movements[0].Type);
        }
    }
}