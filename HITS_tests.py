#!/usr/bin/env python
# script to test your k-means algorithm
import unittest
import HITS

tiny_corpus = [
    {
        "artist":"A",
        "similar":["B","C"],
        "targets":["C"]
    },
    {
        "artist":"B",
        "similar":["C","D"],
        "targets":["A"]
    },
    {
        "artist":"C",
        "similar":["A","D"],
        "targets":["A","B","D"]
    },
    {
        "artist":"D",
        "similar":["C"],
        "targets":["B","C"]
    }
]

class TestClustering(unittest.TestCase):
    def setUp(self):
        self.tester = HITS.HITS()
        for entry in iter(tiny_corpus):
            self.tester.init_artist(entry["artist"], entry["similar"], entry["targets"])

    def test_init(self):
        self.assertEqual(self.tester.hub_score, {'A': 1, 'B': 1, 'C': 1, 'D': 1})
        self.assertEqual(self.tester.auth_score, {'A': 1, 'B': 1, 'C': 1, 'D': 1})
            
    def test_update(self):
        self.tester.calc_update()
        
        self.assertAlmostEqual(self.tester.hub_score['A'], 0.5547, 4)
        self.assertAlmostEqual(self.tester.hub_score['B'], 0.5547, 4)
        self.assertAlmostEqual(self.tester.hub_score['C'], 0.5547, 4)
        self.assertAlmostEqual(self.tester.hub_score['D'], 0.2774, 4)
        
        self.assertAlmostEqual(self.tester.auth_score['A'], 0.2582, 4)
        self.assertAlmostEqual(self.tester.auth_score['B'], 0.2582, 4)
        self.assertAlmostEqual(self.tester.auth_score['C'], 0.7746, 4)
        self.assertAlmostEqual(self.tester.auth_score['D'], 0.5164, 4)

        self.tester.calc_update()
        
        self.assertAlmostEqual(self.tester.hub_score['A'], 0.5208, 4)
        self.assertAlmostEqual(self.tester.hub_score['B'], 0.6509, 4)
        self.assertAlmostEqual(self.tester.hub_score['C'], 0.3906, 4)
        self.assertAlmostEqual(self.tester.hub_score['D'], 0.3906, 4)
        
        self.assertAlmostEqual(self.tester.auth_score['A'], 0.2857, 4)
        self.assertAlmostEqual(self.tester.auth_score['B'], 0.2857, 4)
        self.assertAlmostEqual(self.tester.auth_score['C'], 0.7143, 4)
        self.assertAlmostEqual(self.tester.auth_score['D'], 0.5714, 4)
        
        self.tester.calc_update()
        
        self.assertAlmostEqual(self.tester.hub_score['A'], 0.5065, 4)
        self.assertAlmostEqual(self.tester.hub_score['B'], 0.6512, 4)
        self.assertAlmostEqual(self.tester.hub_score['C'], 0.4341, 4)
        self.assertAlmostEqual(self.tester.hub_score['D'], 0.3618, 4)
        
        self.assertAlmostEqual(self.tester.auth_score['A'], 0.1965, 4)
        self.assertAlmostEqual(self.tester.auth_score['B'], 0.2620, 4)
        self.assertAlmostEqual(self.tester.auth_score['C'], 0.7861, 4)
        self.assertAlmostEqual(self.tester.auth_score['D'], 0.5241, 4)
        
        self.tester.calc_update()

        self.assertAlmostEqual(self.tester.hub_score['A'], 0.5272, 4)
        self.assertAlmostEqual(self.tester.hub_score['B'], 0.6590, 4)
        self.assertAlmostEqual(self.tester.hub_score['C'], 0.3625, 4)
        self.assertAlmostEqual(self.tester.hub_score['D'], 0.3954, 4)
        
        self.assertAlmostEqual(self.tester.auth_score['A'], 0.2189, 4)
        self.assertAlmostEqual(self.tester.auth_score['B'], 0.2554, 4)
        self.assertAlmostEqual(self.tester.auth_score['C'], 0.7663, 4)
        self.assertAlmostEqual(self.tester.auth_score['D'], 0.5474, 4)
        
    def test_HITS(self):
        self.tester.calc_HITS()
        
        self.assertAlmostEqual(self.tester.hub_score['A'], 0.5255, 4)
        self.assertAlmostEqual(self.tester.hub_score['B'], 0.6603, 4)
        self.assertAlmostEqual(self.tester.hub_score['C'], 0.3627, 4)
        self.assertAlmostEqual(self.tester.hub_score['D'], 0.3953, 4)
        
        self.assertAlmostEqual(self.tester.auth_score['A'], 0.1759, 4)
        self.assertAlmostEqual(self.tester.auth_score['B'], 0.2681, 4)
        self.assertAlmostEqual(self.tester.auth_score['C'], 0.8001, 4)
        self.assertAlmostEqual(self.tester.auth_score['D'], 0.5069, 4)
        
if __name__ == '__main__':
    unittest.main()

