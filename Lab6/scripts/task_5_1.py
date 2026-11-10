#!/usr/bin/env python3
"""Завдання 5-1: конвеєрна система з 5 пристроями.
Логи: data/5-1/raw/1.txt, 2.txt, ...
"""
from common import run_task


def calc(pm, px, tm, T):
    N, Nobr, Npov = int(px["Generated"]), int(px["Processed"]), int(px["Returned"])
    rho = [1 - pm[f"Available {i}"] for i in range(1, 6)]            # завантаження пристроїв
    k = sum(tm[f"Processing {i}"] for i in range(1, 6))              # середня кількість зайнятих
    L = (sum(pm[f"Position {i}"] for i in range(1, 6))               # середня кількість деталей у системі
         + sum(tm[f"Moving to {i}"] for i in range(2, 6))
         + tm["Returning to 1"] + k)
    X = Nobr / T                                                     # пропускна здатність
    Tsys = L / X if X else float("nan")                              # закон Літтла
    return [N, Nobr, Npov, Tsys, *rho, sum(rho), k, L, X]


# Інтенсивність e (дет/хв) для прогонів, де вона не дорівнює 4 (за таблицею експериментів)
E_OVERRIDE = {4: 0.01, 5: 0.5, 6: 1, 7: 10, 8: 2}


def check(run, px, T):
    """N має дорівнювати e·T. Інакше час моделювання для цього прогону вказано хибно."""
    e = E_OVERRIDE.get(run, 4)
    expected = e * T
    N = px["Generated"]
    if abs(N - expected) > max(0.01 * expected, 2):
        return (f"N = {N:.0f}, а e·T = {expected:g}. Імовірно, час моделювання інший "
                f"(X та T будуть неправильні). Перевірте default_t/override_t.")
    return None


TASK = dict(
    name="5-1",
    headers=["N", "Nобр", "Nпов", "T", "ρ1", "ρ2", "ρ3", "ρ4", "ρ5", "Σρ", "k", "L", "X"],
    default_t=10000,
    override_t={7: 500, 9: 2500, 18: 100000},
    calc=calc,
    check=check,
    # порядок рядків вихідних даних у таблиці (None = залишити порожнім)
    table_rows=["N", "Nобр", "Nпов", "L", "X", "T", "ρ1", "ρ2", "ρ3", "ρ4", "ρ5",
                "Σρ", "k", None],
)

if __name__ == "__main__":
    run_task(TASK)