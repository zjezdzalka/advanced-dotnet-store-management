# Online Shop – Konsolowa aplikacja sklepu internetowego

## 1. Tytuł projektu
**Nazwa aplikacji:** Online Shop  
**Opis:** Konsolowa aplikacja sklepu internetowego z systemem zarządzania użytkownikami i uprawnieniami RBAC.

## 2. Opis projektu

**Cel projektu:**  
Stworzenie bezpiecznej aplikacji konsolowej do zarządzania sklepem internetowym z podziałem ról użytkowników i odpowiednimi uprawnieniami.

**Funkcje aplikacji:**  
- Rejestracja/logowanie użytkowników  
- Składanie zamówień  
- Zarządzanie produktami i użytkownikami  
- Logowanie aktywności

**Grupy użytkowników:**  
- Klienci sklepu  
- Menedżerowie  
- Administratorzy systemu

## 3. Technologie

- **Język:** C#  
- **Środowisko:** .NET 8.0  
- **IDE:** Visual Studio / Visual Studio Code  
- **Biblioteki:**  
  - `System.Security.Cryptography` – hashowanie haseł  
  - `System.Timers` – obsługa limitu czasu płatności

## 4. Struktura katalogów

```
Program.cs
Classes/
  └── User.cs
Services/
  ├── RBAC.cs
  └── UserManager.cs
Utils/
  └── FileLogger.cs
users.txt
products.txt
orders.txt
logs.txt
```

## 5. Instrukcja instalacji i uruchomienia

**Wymagania:**  
- .NET 8.0 lub nowszy  
- System operacyjny z dostępem do systemu plików

**Instrukcje:**  
1. Pobierz projekt (zalecany Visual Studio)  
2. Uruchom `projektOOP.exe` z folderu `projektOOP/bin/Debug`

## 6. Opis działania aplikacji

**Funkcjonalności:**  
- Rejestracja i logowanie  
- Przegląd i składanie zamówień  
- Zarządzanie użytkownikami i logami (dla admina)  
- System ról: Administrator, Manager, User

**Przebieg działania:**  
- Inicjalizacja plików  
- Logowanie/rejestracja  
- Menu zależne od roli  
- Składanie zamówienia z weryfikacją i płatnością

**Dane przetwarzane:**  
- Dane użytkowników, produkty, zamówienia, logi

## 7. Zrzuty ekranu
_(Do uzupełnienia przy dodaniu grafik)_

## 8. Przykłady użycia

1. Użytkownik wybiera opcję „Rejestracja”  
2. Tworzy konto, loguje się, widzi menu roli „User”  
3. Przegląda produkty, wybiera i opłaca zamówienie  
4. Wylogowuje się

## 9. Struktury danych i klasy

**User:**  
- Właściwości: `Username`, `HashedPassword`, `Role`, `Permissions`, `CreationDate`  
- Metody: `ShowMenu()`, `PlaceOrder()`, `MakePayment()`

**RBAC:**  
- `HasPermission()`, `ApplyPermissions()`

**UserManager:**  
- `Login()`, `Register()`, `UserAction` (zdarzenie)

**FileLogger:**  
- `GetLogs()`, logowanie do pliku

**Enumeratory:**  
- Role: `Administrator`, `Manager`, `User`  
- Permissions: `ViewUsers`, `ManageUsers`, `PlaceOrder`, ...

## 10. Obsługa błędów

- Blokada po 3 nieudanych próbach logowania  
- Anulowanie płatności po 10 minutach braku aktywności  
- Walidacja danych wejściowych (np. numer karty)  
- Obsługa wyjątków przy odczycie/zapisie plików

## 11. Testowanie

- Testowanie manualne  
- Brak testów jednostkowych (możliwość użycia np. xUnit)

## 12. Problemy i ograniczenia

- Brak GUI  
- Brak szyfrowania plików  
- Brak testów jednostkowych

## 13. Plany rozwoju

- Dodanie GUI (np. WPF)  
- Rozszerzenie bazy produktów/zamówień  
- Integracja z bazą danych (np. SQLite)  
- Testy jednostkowe

## 14. Autorzy

- Marek Zjeżdżałka  
- Ernest Płonka
