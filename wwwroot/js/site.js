$(document).ready(function () {
    $('.datatable').DataTable();
});

function CanInvite() {
    swal("Good news!", "Your invitation has been sent.", "success");
}

function CantInvite() {
    swal("Sorry!", "You can't invite someone who is already in a household.", "error");
}

function CantLeave() {
    swal("Sorry!", "You can't leave while the house still has members.", "error");
}

function BudgetBreakdownChart(Url) {
    let names = [];
    let totals = [];
    $.post(Url).then(function (res) {
        console.log(res);
        for (let i = 0; i < res.length; i++) {
            names.push(res[i].name);
            totals.push(res[i].total);
        }
    }).then(() => {
        // For a pie chart
        var ctx = document.getElementById('breakdownPieChart').getContext('2d');
        var myPieChart = new Chart(ctx, {
            type: 'pie',
            data: {
                datasets: [{
                    data: totals,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                    ],
                    borderWidth: 1
                }],
                labels: names
            }
        });
    });
}

function CategoryItemsChart(Url) {
    let labels = [];
    let goal = [];
    let reality = [];
    $.post(Url).then(function (res) {
        console.log(res);
        for (let i = 0; i < res.length; i++) {
            labels.push(res[i].name);
            goal.push(res[i].goal);
            reality.push(res[i].reality);
        }
    }).then(() => {
        var ctx = document.getElementById('itemsBarChart').getContext('2d');
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'budget',
                    data: goal,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.1)',
                        'rgba(54, 162, 235, 0.1)',
                        'rgba(255, 206, 86, 0.1)',
                        'rgba(75, 192, 192, 0.1)',
                        'rgba(153, 102, 255, 0.1)',
                        'rgba(255, 159, 64, 0.1)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                },
                {
                    label: 'spent',
                    data: reality,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.5)',
                        'rgba(54, 162, 235, 0.5)',
                        'rgba(255, 206, 86, 0.5)',
                        'rgba(75, 192, 192, 0.5)',
                        'rgba(153, 102, 255, 0.5)',
                        'rgba(255, 159, 64, 0.5)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });
    })
}
