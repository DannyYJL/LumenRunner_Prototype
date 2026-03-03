import csv
from collections import Counter
import matplotlib.pyplot as plt
from pathlib import Path

CSV_PATH = Path("death_log.csv")             # 输入CSV文件
OUTPUT_PNG = Path("death_by_cause.png")      # 输出图片
CHART_TITLE = "Death by Cause Distribution"  # 图标题

def main():
    if not CSV_PATH.exists():
        raise FileNotFoundError(f"CSV not found: {CSV_PATH.resolve()}")

    counts = Counter()

    with CSV_PATH.open(newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        # 兼容列名大小写差异
        for row in reader:
            cause = (row.get("cause") or row.get("Cause") or "Other").strip()
            if cause == "":
                cause = "Other"
            counts[cause] += 1

    if not counts:
        print("No rows found in CSV (or 'cause' column missing).")
        return

    labels = list(counts.keys())
    values = [counts[k] for k in labels]

    plt.figure()
    plt.title(CHART_TITLE)

    # autopct 显示百分比
    plt.pie(values, labels=labels, autopct="%1.1f%%", startangle=90)
    plt.axis("equal")  # 保证是圆形
    plt.tight_layout()

    plt.savefig(OUTPUT_PNG, dpi=200)
    plt.close()

    print("Saved pie chart to:", OUTPUT_PNG.resolve())
    print("Counts:", dict(counts))

if __name__ == "__main__":
    main()