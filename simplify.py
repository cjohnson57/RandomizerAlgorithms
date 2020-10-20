import boolean
import sys

# This script simplifies a boolean logic expression
# So input "A or A and B" will give output "A"
# Used in calculation of world complexity
algebra = boolean.BooleanAlgebra()
test = algebra.parse(sys.argv[1])
print(test.simplify()) # Redirected to a string in the C#
