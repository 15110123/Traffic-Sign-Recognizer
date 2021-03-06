﻿using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Trainers.KMeans;
using System.Collections.Generic;
using System.Linq;

namespace TrafficSignRecognizer.API.Models
{
    public class KMeansClusterModel<TSrc, TDst> 
        where TSrc : class 
        where TDst : class, new()
    {
        private KMeansPlusPlusTrainer _trainer;
        public int ClusterCount { get; private set; }
        private MLContext _mLContext;
        private ITransformer _transformer;
        private EstimatorChain<ClusteringPredictionTransformer<KMeansPredictor>> _pipeline;
        private PredictionFunction<TSrc, TDst> _predictionFunction;
        private IDataView _trainData;

        public KMeansClusterModel(int clusterCount)
        {
            ClusterCount = clusterCount;
            _mLContext = new MLContext();
            //_trainer = new KMeansPlusPlusTrainer(_mLContext, "Quantity", clusterCount);
            _trainer = _mLContext.Clustering.Trainers.KMeans("Quantity", clustersCount: clusterCount);
            _pipeline = _mLContext.Transforms.Concatenate("Quantity", "Quantity")
                .Append(_trainer);
        }

        public void Train(IEnumerable<TSrc> trainInput)
        {
            _trainData = _mLContext.CreateStreamingDataView(trainInput);
            _transformer = _pipeline.Fit(_trainData);
        }

        public TDst Predict(TSrc predictInput)
        {
            return _transformer.MakePredictionFunction<TSrc, TDst>(_mLContext).Predict(predictInput);
        }
    }
}
