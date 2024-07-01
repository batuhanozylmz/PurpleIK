const plans = document.querySelectorAll('.plan');
const selectPlanButton = document.getElementById('selectPlan');

let selectedPlan = null;

plans.forEach(plan => {
    plan.addEventListener('click', () => {
        deselectAllPlans();
        selectPlan(plan);
    });
});

selectPlanButton.addEventListener('click', () => {
    if (selectedPlan) {
        alert(`You selected the ${selectedPlan.dataset.users} user plan with ${selectedPlan.dataset.resolution} resolution for ${selectedPlan.dataset.price} TL/month.`);
    } else {
        alert('Please select a plan.');
    }
});

function deselectAllPl