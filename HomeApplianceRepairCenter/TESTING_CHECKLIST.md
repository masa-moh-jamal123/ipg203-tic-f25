# Manual Testing Checklist

Use this file before submitting the project.

## 1. Refrigerator
- Register a refrigerator with a unique serial number.
- Try an invalid serial first (for example `A!`) and verify it is rejected.
- Verify its cost includes refrigerator-specific complexity/refill factors.

## 2. Washing Machine
- Register a washing machine.
- Verify load capacity is restricted to 1-30 kg.
- Verify its cost differs from the refrigerator formula.

## 3. Air Conditioner
- Register an air conditioner.
- Verify BTU is restricted to 5000-60000.
- Verify its cost uses the AC-specific formula.

## 4. Encapsulation
- Confirm SerialNumber and ReceivedDate cannot be changed from Program.
- Confirm Status has no public setter and changes only through ChangeStatus().

## 5. Valid lifecycle
- Received -> Diagnosing
- Diagnosing -> InRepair
- InRepair -> Repaired
- Repaired -> Delivered

## 6. Invalid lifecycle
- Attempting an invalid transition in code (for example Received -> Delivered) must throw InvalidOperationException.
- The console menu only displays allowed next statuses.

## 7. Events
- On every valid status change, verify `[EVENT]` is printed.
- On transition to Repaired, verify `[REPAIR COMPLETED EVENT]` is printed.

## 8. Static property
- Register three devices: the count should be 3 under maintenance.
- Mark one as Repaired: the static count should become 2.

## 9. Polymorphism
- Add all three appliance types to the center.
- Display all / statistics.
- Verify RepairCenter works on `Appliance` references while each subtype returns its own repair cost.

## 10. Final submission checks
- Build in Release mode.
- Fix all warnings (TreatWarningsAsErrors is enabled).
- Verify the report uses the exact class/method/property names from this project.
- Add the real GitHub repository link only after the repository is published.
