"""Спільний код для завдань лабораторної 6: розбір логів PetriNetSim і запис CSV.

Структура:
    lab6/scripts/common.py, task_*.py
    lab6/data/<завдання>/raw/1.txt, 2.txt, ...
    lab6/data/<завдання>/results.csv         (прогони = рядки, з заголовками)
    lab6/data/<завдання>/results_table.tsv   (прогони = стовпці, у порядку рядків таблиці; для вставки)
"""
import re, csv, sys, argparse
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent      # lab6/
DATA_DIR = BASE_DIR / "data"


def parse(text):
    """Повертає (середні по місцях, max по місцях, середні по переходах)."""
    place_mean, place_max, trans_mean = {}, {}, {}
    cur = None
    for line in text.splitlines():
        m = re.match(r"\s*Place\s+(.+?):\s+mean value\s*=\s*([-\d.eE+]+)", line)
        if m:
            cur = m.group(1).strip()
            place_mean[cur] = float(m.group(2))
            continue
        m = re.match(r"\s*Transition\s+(.+?)\s+has mean value\s+([-\d.eE+]+)", line)
        if m:
            cur = None
            trans_mean[m.group(1).strip()] = float(m.group(2))
            continue
        m = re.match(r"\s*max value\s*=\s*([-\d.eE+]+)", line)
        if m and cur:
            place_max[cur] = float(m.group(1))
            continue
        m = re.match(r"\s*min value\s*=\s*([-\d.eE+]+)", line)
        if m and cur:
            place_max[cur + "::min"] = float(m.group(1))   # мінімум місця зберігаємо під ключем "<назва>::min"
    return place_mean, place_max, trans_mean


def num_key(p):
    m = re.search(r"(\d+)", p.stem)
    return int(m.group(1)) if m else 10**9


def run_task(task):
    """task: name, headers, default_t, override_t, calc(pm, px, tm, T) -> список значень,
    необов'язково check(run, px, T) -> текст попередження або None,
    необов'язково table_rows — порядок рядків вихідних даних у таблиці (None = порожній рядок)."""
    ap = argparse.ArgumentParser(description=f"Обробка логів завдання {task['name']}")
    ap.add_argument("--data", default=str(DATA_DIR / task["name"]),
                    help=f"папка завдання (за замовчуванням data/{task['name']}; логи беруться з raw/)")
    ap.add_argument("--ua", action="store_true",
                    help="роздільник ';' і десяткова кома (українська локалізація)")
    a = ap.parse_args()

    data = Path(a.data)
    raw = data / "raw"
    files = sorted(raw.glob("*.txt"), key=num_key)
    if not files:
        sys.exit(f"У папці {raw} немає файлів *.txt")

    rows = []
    for f in files:
        n = num_key(f)
        T = task["override_t"].get(n, task["default_t"])
        pm, px, tm = parse(f.read_text(encoding="utf-8", errors="ignore"))
        try:
            vals = task["calc"](pm, px, tm, T, run=n) if task.get("needs_run") else task["calc"](pm, px, tm, T)
            rows.append([n, T, *vals])
        except KeyError as e:
            sys.exit(f"{f.name}: у логу немає очікуваного елемента {e}. Перевірте назви місць і переходів.")
        print(f"{f.name}: прогін {n}, T мод = {T:g}")
        if task.get("check"):
            warn = task["check"](n, px, T)
            if warn:
                print(f"   !! УВАГА: {warn}")

    def fmt(v):
        if isinstance(v, str):
            return v
        if isinstance(v, int):
            return str(v)
        t = f"{v:.6g}"
        return t.replace(".", ",") if a.ua else t

    out = data / "results.csv"
    with open(out, "w", encoding="utf-8-sig", newline="") as fh:
        w = csv.writer(fh, delimiter=";" if a.ua else ",")
        w.writerow(["run", "T_мод", *task["headers"]])
        for r in rows:
            w.writerow([fmt(v) for v in r])
    print("Готово:", out)

    # Транспонований файл для вставки в таблицю: рядки = показники в порядку таблиці, стовпці = прогони
    layout = task.get("table_rows")
    if layout:
        col = {h: i for i, h in enumerate(task["headers"])}
        lines = []
        for name in layout:
            if name is None or name not in col:
                lines.append("\t".join("" for _ in rows))
            else:
                lines.append("\t".join(fmt(r[2 + col[name]]) for r in rows))
        out_t = data / "results_table.tsv"
        out_t.write_text("\n".join(lines) + "\n", encoding="utf-8")
        print("Для вставки в таблицю:", out_t)