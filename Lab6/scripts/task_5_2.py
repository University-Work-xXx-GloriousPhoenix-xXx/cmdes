#!/usr/bin/env python3
"""Завдання 5-2: система управління запасами холодильників.
Логи: data/5-2/raw/1.txt, 2.txt, ...   Запуск:  python scripts/task_5_2.py --ua
Результат: data/5-2/results.csv та data/5-2/results_table.tsv (рядки в порядку таблиці, прогони — стовпці).
Параметри кожного прогону (μ, p, t, d, S, s, L, T) мають збігатися з таблицею — змінюйте їх у PARAMS.
"""
import math
from common import run_task

# прогін: (μ, p, t, d, S, s, L, T)   p — імовірність чекати
PARAMS = {
    1: (5, 0.2, 4, 3, 72, 18, 72, 5000),
    2: (5, 0.2, 4, 3, 72, 18, 72, 5000),
    3: (5, 0.2, 4, 3, 72, 18, 72, 5000),
    4: (10, 0.2, 4, 3, 72, 18, 72, 5000),
    5: (1, 0.2, 4, 3, 72, 18, 72, 5000),
    6: (5, 1.0, 4, 3, 72, 18, 72, 5000),
    7: (5, 0.0, 4, 3, 72, 18, 72, 5000),
    8: (5, 0.2, 2, 3, 72, 18, 72, 5000),
    9: (5, 0.2, 8, 3, 72, 18, 72, 5000),
    10: (5, 0.2, 4, 1, 72, 18, 72, 5000),
    11: (5, 0.2, 4, 6, 72, 18, 72, 5000),
    12: (5, 0.2, 4, 3, 72, 36, 72, 5000),
    13: (5, 0.2, 4, 3, 90, 18, 72, 5000),
    14: (5, 0.2, 4, 3, 72, 18, 36, 5000),
    15: (5, 0.2, 4, 3, 72, 18, 72, 2500),
}


def sanity(run, pm, px, T, mu, p, S, s, N_clients, left, queued, sold_tot, served, avg_order, dl, lit):
    """Перевірки, які виводяться в консоль (у таблицю не потрапляють). Повертає список попереджень."""
    w = []
    # 1. Покупці: Total clients ≈ μ·T (допуск 4σ пуассонівського потоку)
    exp_n = mu * T
    if abs(N_clients - exp_n) > 4 * math.sqrt(exp_n):
        w.append(f"Total clients = {N_clients:.0f}, а μ·T = {exp_n:g}")
    # 2. Баланс: покупці = обслужені + пішли + ті, що ще чекають у черзі (0 … max Queue)
    rest = N_clients - served - left
    if rest < 0 or rest > px.get("Queue", 0):
        w.append(f"Total clients − Served − Left = {rest:.0f}, очікується 0…{px.get('Queue', 0):.0f} (черга на кінець)")
    # 3. Total sold рахує прямі й відкладені продажі; різниця з покупцями — це Sold, що ще не злитий (0 … max Sold)
    rest2 = N_clients - sold_tot - left
    if rest2 < 0 or rest2 > px.get("Sold", 0):
        w.append(f"Total clients − Total sold − Left = {rest2:.0f}, очікується 0…{px.get('Sold', 0):.0f}")
    # 4. Частка тих, хто пішов ≈ 1 − p (4σ біноміального розкиду); при p = 0 і p = 1 — точно
    n = left + queued
    if p == 1 and left != 0:
        w.append(f"p = 1 (усі чекають), але Left = {left:.0f}")
    elif p == 0 and queued != 0:
        w.append(f"p = 0 (ніхто не чекає), але Queued = {queued:.0f}")
    elif 0 < p < 1 and n > 0 and abs(left / n - (1 - p)) > 4 * math.sqrt(p * (1 - p) / n):
        w.append(f"частка тих, хто пішов = {left / n:.3f}, очікується {1 - p:g}")
    # 5. Закон Літтла для дороги: Deliver, mean ≈ Total sold / T · d
    if lit > 0 and abs(dl - lit) / lit > 0.02:
        w.append(f"Deliver, mean = {dl:.3g}, а Total sold / T · d = {lit:.3g}")
    # 6. Запас: мінімум ≥ 0, максимум ≤ max(S, L) + макс. довжина черги (на момент доставки фішки з черги забираються миттєво)
    zmax = px["Fridges"]; zmin = px.get("Fridges::min", 0)
    if zmax > max(S, PARAMS[run][6]) + px.get("Queue", 0):
        w.append(f"макс. запас {zmax:.0f} > max(S, L) + макс. черга = {max(S, PARAMS[run][6]) + px.get('Queue', 0):.0f}")
    # 7. Пріоритет Serve: продаж не втрачається, поки запас > 0
    if left > 0 and zmin >= 1:
        w.append("Left > 0, але запас ніколи не був нульовим: у Serve немає пріоритету над Leave/To queue")
    # 8. Обсяг замовлення не менший за S − s
    if isinstance(avg_order, float) and avg_order < (S - s) - 1e-9:
        w.append(f"середній обсяг замовлення {avg_order:.1f} < S − s = {S - s}")
    return w


def calc(pm, px, tm, T, run):
    mu, p, t, d, S, s, L, T_tab = PARAMS[run]
    if abs(T - T_tab) > 1e-9:
        print(f"   !! УВАГА: T для прогону {run}: {T:g} (скрипт) ≠ {T_tab:g} (PARAMS)")
    fm = pm["Fridges"]
    fmm = f"{px.get('Fridges::min', 0):.0f} / {px['Fridges']:.0f}"
    left, queued = int(px["Left"]), int(px["Queued"])
    sold_tot, canc = int(px["Total sold"]), int(px["Cancelled"])
    dl = tm["Deliver"]
    tau = T / left if left else "—"
    share = left / (left + queued) if left + queued else "—"
    N = sold_tot + left
    orders = int(T // t) - canc
    avg = sold_tot / orders if orders > 0 else "—"
    lit = sold_tot / T * d
    for msg in sanity(run, pm, px, T, mu, p, S, s, px["Total clients"], left, queued, sold_tot,
                      px["Served"], avg, dl, lit):
        print(f"   !! УВАГА: {msg}")
    return [fm, fmm, left, queued, sold_tot, canc, dl, tau, share, N, orders, avg, lit]


HEADERS = ["Fridges mean", "Fridges min / max", "Left", "Queued", "Total sold", "Cancelled", "Deliver mean",
           "T/Left", "Left/(Left+Queued)", "Total sold + Left", "Кількість замовлень",
           "Total sold / замовлення", "Total sold / T · d"]

TASK = dict(
    name="5-2",
    headers=HEADERS,
    default_t=5000,
    override_t={15: 2500},
    calc=calc,
    needs_run=True,
    table_rows=HEADERS,
)

if __name__ == "__main__":
    run_task(TASK)