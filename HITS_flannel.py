import os
import sys
import glob
import time
import datetime
import numpy as np

import HITS

try:
	import sqlite3
except ImportError:
	print 'you need sqlite3 installed to run this program'
	sys.exit(0)

def generate_corpus():
	dbfile = "/home/kdwebb/Documents/artist_similarity.db"

	conn = sqlite3.connect(dbfile)
	c = conn.cursor()

	q = "SELECT artist_id FROM artists"
	res = c.execute(q)
	artists = res.fetchall()

	corpus = []
	for artist in artists:
		d = {}
		d["artist"] = repr(str(artist[0]))

		q = "SELECT similar FROM similarity WHERE target=" + d["artist"]
		res = c.execute(q)
		d["similar"] = []
		for sim in res.fetchall():
			d["similar"].append(repr(str(sim[0])))

		q = "SELECT target FROM similarity WHERE similar=" + d["artist"]
		res = c.execute(q)
		d["targets"] = []
		for tar in res.fetchall():
			d["targets"].append(repr(str(tar[0])))

		corpus.append(d)

	conn.close()
	
	return corpus

hits = HITS.HITS()

print "generating corpus"
corpus = generate_corpus()

print "initializing HITS"
for entry in iter(corpus):
	hits.init_artist(entry["artist"],entry["similar"],entry["targets"])

print "calculating HITS"
hits.calc_HITS()

print "saving results to database"
try:
	os.remove("hits.db")
except OSError:
	pass
conn = sqlite3.connect("hits.db")
c = conn.cursor()
c.execute('''CREATE TABLE hits
		(artist_id text, hub real, auth real)''')
for entry in iter(corpus):
	q = "INSERT INTO hits VALUES (" + entry["artist"] + "," + str(hits.hub_score[entry["artist"]]) + "," + str(hits.auth_score[entry["artist"]]) + ")";
	c.execute(q)
conn.commit()

q = "SELECT hub FROM hits WHERE artist_id='ARZZZKG1271F573BC4'"
res = c.execute(q)
print res.fetchone()[0]
print hits.hub_score["'ARZZZKG1271F573BC4'"]

conn.close()

