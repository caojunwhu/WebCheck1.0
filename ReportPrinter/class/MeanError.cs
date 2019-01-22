using System.Collections.Generic;
using DatabaseDesignPlus;

namespace ReportPrinter
{

    public class PositionMeanError : MeanError, IMeanError
	{
        public PositionMeanError(Dictionary<string, string> Configs, Dictionary<string, string> Databases):base(Configs,Databases)
        {
            this.sErrorType = "平面绝对位置误差";
            this.Configs = Configs;
            this.Databases = Databases;

            _ErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["CheckPointsDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);


            _MeanErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                      "PostgreSQL",

                    Configs["CheckPointsMeanErrorDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["CheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["PositonMeanErrorDataTableName"]);
        }
        public PositionMeanError()
        {
            _ErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                      "PostgreSQL",

                    Configs["CheckPointsDataTableDesignName"],
                    Configs["DataTableDesignVersion"], 
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _MeanErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                      "PostgreSQL",

                    Configs["CheckPointsMeanErrorDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["CheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["PositonMeanErrorDataTableName"]);

        }

        public new void Calc(string sMapNumber)
        {
            if (nValidErrorCount == 0)
            {
                vError = vScore = nGrossErrorRatio = -1;
            }
            else
            {
                vError = nValidErrorCount >= 20 ? vError * 1000 / vScale : vAvgError * 1000 / vScale;
                vScore = CalculateMathScore(vMaxError, vError, vError);
                nGrossErrorRatio = nValidErrorCount >= 20 ? nGrossErrorRatio : -1;
            }
        }

        //将sMapnumber图幅的平面精度得分/粗差率情况/描述等写入样本质量打分表中
        public new void UpdateScore(string sMapnumber)
        {
            

        }

	}

    public class HeightMeanError : MeanError, IMeanError
    {
        public HeightMeanError(Dictionary<string, string> Configs, Dictionary<string, string> Databases)
            : base(Configs, Databases)
        {
            this.sErrorType = "高程绝对位置误差";
            this.Configs = Configs;
            this.Databases = Databases;

            _ErrorDataTableDesign = new DataTableDesign(
                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",

                Configs["CheckPointsDataTableDesignName"],
                Configs["DataTableDesignVersion"],
                Configs["DataTableDesignFieldAttributesName"],

                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",
                Configs["DatatableDataTypeRelationDataTableName"]);

            _MeanErrorDataTableDesign = new DataTableDesign(
                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",

                Configs["CheckPointsMeanErrorDataTableDesignName"],
                Configs["DataTableDesignVersion"],
                Configs["DataTableDesignFieldAttributesName"],

                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",
                Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["CheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["HeightMeanErrorDataTableName"]);

        }

        public HeightMeanError()
        {
            _ErrorDataTableDesign = new DataTableDesign(
                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",

                Configs["CheckPointsDataTableDesignName"],
                Configs["DataTableDesignVersion"],
                Configs["DataTableDesignFieldAttributesName"],

                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",
                Configs["DatatableDataTypeRelationDataTableName"]);

            _MeanErrorDataTableDesign = new DataTableDesign(
                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",

                Configs["CheckPointsMeanErrorDataTableDesignName"],
                Configs["DataTableDesignVersion"],
                Configs["DataTableDesignFieldAttributesName"],

                Databases["SurveryProductCheckDatabase"],
                "PostgreSQL",
                Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["CheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["HeightMeanErrorDataTableName"]);

        }

        public new void Calc(string sMapNumber)
        {
            if (nValidErrorCount == 0)
            {
                vError = vScore = nGrossErrorRatio = -1;
            }
            else
            {
                vError = nValidErrorCount >= 20 ? vError : vAvgError;
                vScore = CalculateMathScore(vMaxError, vError, vError);
                nGrossErrorRatio = nValidErrorCount >= 20 ? nGrossErrorRatio : -1;
            }
        }
        
      }


    public class RelativeMeanError : MeanError, IMeanError
    {
        public RelativeMeanError(Dictionary<string, string> Configs, Dictionary<string, string> Databases)
            : base(Configs, Databases)
        {
            this.sErrorType = "平面相对位置误差";
            this.Configs = Configs;
            this.Databases = Databases;

            _ErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["RelativeCheckPointsTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _MeanErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["CheckPointsMeanErrorDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["RelativeCheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["RelativeMeanErrorDataTableName"]);

        }

        public RelativeMeanError()
        {
            _ErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["RelativeCheckPointsTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _MeanErrorDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["CheckPointsMeanErrorDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["RelativeCheckPointsTableName"]);
            _MeanErrorDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["RelativeMeanErrorDataTableName"]);
        }


        public new void Calc(string sMapNumber)
        {
            if (nValidErrorCount == 0)
            {
                vError = vScore = nGrossErrorRatio = -1;
            }
            else
            {
                vError = nValidErrorCount >= 20 ? vError * 1000 / vScale : vAvgError * 1000 / vScale;
                vScore = CalculateMathScore(vMaxError, vError, vError);
                nGrossErrorRatio = nValidErrorCount >= 20 ? nGrossErrorRatio : -1;
            }
        }

    }
}
