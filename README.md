# RAGEMP-NAPI-Task-Wrapper

A lightweight task wrapper for [RAGE:MP](https://rage.mp/) that helps safely execute logic in the main thread using `NAPI.Task`.

## ✅ Purpose

This wrapper solves a common problem in RAGE:MP when calling `NAPI` functions (e.g. manipulating vehicles, players) from background threads — which can lead to crashes or undefined behavior.

✅ It includes an internal check to **avoid unnecessary transitions** to the main thread if already inside it.

It provides:
- Safe main-thread execution via `SafeRun()`
- Easy return to main thread after `async/await` via `ReturnToMainThread()`

## 🔧 Usage

### 1. Run NAPI logic in the main thread
```csharp
NAPI.Task.SafeRun(() => NAPI.Entity.SetEntityDimension(entity, 0));
```

### 2. Run function and return result safely
```csharp
var dimension = await NAPI.Task.SafeRun(() => NAPI.Entity.GetEntityDimension(entity));
```

### 3. Return to main thread after async operations
```csharp
await NAPI.Task.ReturnToMainThread();
```

### 4. Await async DB call, then return to main thread
```csharp
var vehicle = await vehicleRepository.FindByNumber("ABC123")
                                     .ReturnToMainThread();

NAPI.Vehicle.SetVehicleNumberPlate(vehicleHandle, vehicle.Number);
```

## 🔌 Extensions included

- `SafeRun(Action)`
- `SafeRun<T>(Func<T>)`
- `Task.ReturnToMainThread()`
- `Task<T>.ReturnToMainThread()`

## 💡 Install

Copy `GtaTaskExtension.cs` into your RAGE:MP server project.
