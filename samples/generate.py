import random
for p in range(1, 4):
	for x in range(1, 10):
		for z in range(1, 3):
			f = open('{}p{}_{}.txt'.format(x, p, z), 'w')
			for randNumb in range(0, x * pow(10, p)):
				f.write('{} '.format(random.randint(0, 30000)))
			f.close