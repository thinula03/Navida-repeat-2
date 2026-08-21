# Manual Test Cases

| ID | Scenario | Input Focus | Expected Result |
| --- | --- | --- | --- |
| TC01 | Low-risk valid proposal | Quantity 1, budget 1500, Sri Lanka | `Approved`, no validation errors, no alerts |
| TC02 | Validation failure | Quantity 0 and budget 0 | `ValidationFailed`, validation errors returned |
| TC03 | Budget rule | Budget 5000.01 | `ReviewRequired`, `BUDGET-001`, Low |
| TC04 | Resource rule | Advanced GPU Cluster, quantity 4 | `ReviewRequired`, `RESOURCE-001`, High |
| TC05 | Ethics rule | Bermuda | `ReviewRequired`, `ETHICS-001`, High |
| TC06 | Combined risk | Budget 9000, GPU quantity 4, Bermuda | `ReviewRequired`, three alerts returned |
