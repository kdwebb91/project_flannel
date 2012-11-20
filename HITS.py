from __future__ import division
import re
import math
from collections import defaultdict
import time
import operator
import itertools

class HITS(object):
    """ A script to calculate the hubs and authorities of Artists """
    def __init__(self):
        self.hub_score = {}
        self.auth_score = {}
        self.hubs = defaultdict(list)
        self.auths = defaultdict(list)

    def init_artist(self, artist, similar, targets):
        self.hub_score[artist] = 1
        self.auth_score[artist] = 1
        self.hubs[artist] = similar
        self.auths[artist] = targets

    def calc_update(self):
        new_hub_score = {}
        new_auth_score = {}
        hub_norm = 0
        auth_norm = 0
        for artist in self.hubs:
            auth_s = 0
            hub_s = 0
            for hub in self.hubs[artist]:
                hub_s = hub_s + self.auth_score[hub]
            for auth in self.auths[artist]:
                auth_s = auth_s + self.hub_score[auth]
            hub_norm += math.pow(hub_s, 2)
            auth_norm += math.pow(auth_s, 2)
            new_hub_score[artist] = hub_s
            new_auth_score[artist] = auth_s
        for artist in self.auths:
            self.hub_score[artist] = new_hub_score[artist]/math.sqrt(hub_norm)
            self.auth_score[artist] = new_auth_score[artist]/math.sqrt(auth_norm)

    def calc_HITS(self):
        old_hub_rank = sorted(self.hub_score.items(), key=operator.itemgetter(1))
        old_auth_rank = sorted(self.auth_score.items(), key=operator.itemgetter(1))
        different = True
        while different:
            different = False
            self.calc_update()
            new_hub_rank = sorted(self.hub_score.items(), key=operator.itemgetter(1))
            new_auth_rank = sorted(self.auth_score.items(), key=operator.itemgetter(1))
            for old, new in itertools.izip(old_hub_rank, new_hub_rank):
                if old[0] is not new[0]:
                    different = True
                    break
            if not different:
                for old, new in itertools.izip(old_auth_rank, new_auth_rank):
                    if old[0] is not new[0]:
                        different = True
                        break
            old_hub_rank = new_hub_rank
            old_auth_rank = new_auth_rank
        
        
        
        
