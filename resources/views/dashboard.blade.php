<!-- resources/views/dashboard.blade.php -->

@extends('layouts.app')

@section('content')
<div class="container-fluid">
    <div class="row">
        <!-- Sidebar -->
        <nav id="sidebar" class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
            <div class="position-sticky">
                <ul class="nav flex-column">
                    <li class="nav-item">
                        <a class="nav-link active" aria-current="page" href="#">
                            <i class="fas fa-tachometer-alt"></i>
                            Dashboard
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">
                            <i class="fas fa-receipt"></i>
                            Invoices
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">
                            <i class="fas fa-money-bill"></i>
                            Expenses
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">
                            <i class="fas fa-chart-line"></i>
                            Reports
                        </a>
                    </li>
                </ul>
            </div>
        </nav>

        <!-- Main content -->
        <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
            <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                <h1 class="h2">Dashboard</h1>
            </div>

            <!-- Summary cards -->
            <div class="row">
                <div class="col-md-3">
                    <div class="card text-white bg-primary mb-3">
                        <div class="card-header">Total Revenue</div>
                        <div class="card-body">
                            <h5 class="card-title">$10,000</h5>
                            <p class="card-text">This is the total revenue for the current month.</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card text-white bg-success mb-3">
                        <div class="card-header">Total Expenses</div>
                        <div class="card-body">
                            <h5 class="card-title">$5,000</h5>
                            <p class="card-text">This is the total expenses for the current month.</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card text-white bg-warning mb-3">
                        <div class="card-header">Net Profit</div>
                        <div class="card-body">
                            <h5 class="card-title">$5,000</h5>
                            <p class="card-text">This is the net profit for the current month.</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card text-white bg-danger mb-3">
                        <div class="card-header">Outstanding Invoices</div>
                        <div class="card-body">
                            <h5 class="card-title">$2,000</h5>
                            <p class="card-text">This is the total amount of outstanding invoices.</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Charts -->
            <div class="row">
                <div class="col-md-6">
                    <div class="card mb-3">
                        <div class="card-header">
                            Revenue Chart
                        </div>
                        <div class="card-body">
                            <canvas id="revenueChart" width="400" height="200"></canvas>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card mb-3">
                        <div class="card-header">
                            Expenses Chart
                        </div>
                        <div class="card-body">
                            <canvas id="expensesChart" width="400" height="200"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Recent transactions table -->
            <div class="row">
                <div class="col-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            Recent Transactions
                        </div>
                        <div class="card-body">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th scope="col">Date</th>
                                        <th scope="col">Description</th>
                                        <th scope="col">Amount</th>
                                        <th scope="col">Category</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>2024-06-01</td>
                                        <td>Invoice #123</td>
                                        <td>$1,000</td>
                                        <td>Revenue</td>
                                    </tr>
                                    <tr>
                                        <td>2024-06-02</td>
                                        <td>Office Supplies</td>
                                        <td>$200</td>
                                        <td>Expense</td>
                                    </tr>
                                    <tr>
                                        <td>2024-06-03</td>
                                        <td>Invoice #124</td>
                                        <td>$2,000</td>
                                        <td>Revenue</td>
                                    </tr>
                                    <tr>
                                        <td>2024-06-04</td>
                                        <td>Utilities</td>
                                        <td>$300</td>
                                        <td>Expense</td>
                                    </tr>
                                    <tr>
                                        <td>2024-06-05</td>
                                        <td>Invoice #125</td>
                                        <td>$3,000</td>
                                        <td>Revenue</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    </div>
</div>

<!-- Include Chart.js -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
    // Revenue Chart
    const ctxRevenue = document.getElementById('revenueChart').getContext('2d');
    const revenueChart = new Chart(ctxRevenue, {
        type: 'line',
        data: {
            labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
            datasets: [{
                label: 'Revenue',
                data: [3000, 4000, 2000, 1000],
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    // Expenses Chart
    const ctxExpenses = document.getElementById('expensesChart').getContext('2d');
    const expensesChart = new Chart(ctxExpenses, {
        type: 'bar',
        data: {
            labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
            datasets: [{
                label: 'Expenses',
                data: [500, 700, 300, 1500],
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
</script>
@endsection
