$(document).ready(function () {
    $('.datatable').DataTable();

    var table = $('#recentTransactions').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'excel', 'pdf'
        ]
    });

    table.buttons().container()
        .appendTo($('.col-sm-6:eq(0)', table.table().container()));

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

function Overdraft() {
    swal("Overdraft!", "Details have been sent to your email.", "error");
}

function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function hexToRgb(hex) {
    var bigint = parseInt(hex, 16);
    var r = (bigint >> 16) & 255;
    var g = (bigint >> 8) & 255;
    var b = bigint & 255;    
    return r + "," + g + "," + b;
}

function BudgetBreakdownChart(Url) {
    let names = [];
    let totals = [];
    let backgroundColors = [];
    let borderColors = [];
    $.post(Url).then(function (res) {
        for (let i = 0; i < res.length; i++) {
            names.push(res[i].name);
            totals.push(res[i].total);
            let hexColor = getRandomColor();
            let rgbColor = hexToRgb(hexColor);
            backgroundColors.push(`rgba(${rgbColor},0.2)`);
            borderColors.push(`rgba(${rgbColor},1)`);
        }
    }).then(() => {
        // For a pie chart
        var ctx = document.getElementById('breakdownPieChart').getContext('2d');
        var myPieChart = new Chart(ctx, {
            type: 'pie',
            data: {
                datasets: [{
                    data: totals,
                    backgroundColor: backgroundColors,
                    borderColor: borderColors,
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

    let bar1Border = [];
    let bar1Background = [];
    let bar2Border = [];
    let bar2Background = [];

    $.post(Url).then(function (res) {
        for (let i = 0; i < res.length; i++) {
            labels.push(res[i].name);
            goal.push(res[i].goal);
            reality.push(res[i].reality);

            let hexColor = getRandomColor();
            let rgbColor = hexToRgb(hexColor);

            bar1Border.push(`rgba(${rgbColor},1)`);
            bar1Background.push(`rgba(${rgbColor},0.1)`);
            bar2Border.push(`rgba(${rgbColor},1)`);
            bar2Background.push(`rgba(${rgbColor},0.5)`);            
        }
        console.log(bar1Background);
        console.log(bar1Border);
        console.log(bar2Background);
        console.log(bar2Border);
    }).then(() => {
        var ctx = document.getElementById('itemsBarChart').getContext('2d');
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'budget',
                    data: goal,
                    backgroundColor: bar1Background,
                    borderColor: bar1Border,
                    borderWidth: 1
                },
                {
                    label: 'spent',
                    data: reality,
                    backgroundColor: bar2Background,
                    borderColor: bar2Border,
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
