N = 10000

def print_decorated(message, padding = 0, newline=False):
    length = len(message) + 2 * padding
    if newline:
        print()
    print("=" * length)
    print(padding * " " + message + padding * " ")
    print("=" * length)