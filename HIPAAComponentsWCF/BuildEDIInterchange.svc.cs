using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HIPAAComponents;

namespace HIPAAComponentsWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BuildEDIInterchange" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BuildEDIInterchange.svc or BuildEDIInterchange.svc.cs at the Solution Explorer and start debugging.
    public class BuildEDIInterchange : IBuildEDIInterchange1
    {
        #region GlobalVariables
        private int intInterchangeID;
        private string strSenderIDQual;
        private string strSenderID;
        private string strReceiverIDQual;
        private string strReceiverID;
        private bool blnAcknowledgement;
        private string strRepetitionSeparator;
        private string strElementSeparator;
        private string strUsage;
        private int intSubmitterEDIContactID;
        private string strContactFunctionCode;
        private string strContactName;
        private string strCommunicationNumberID;
        private string strCommunicationNumber;
        private string strCommunicationNumberID2;
        private string strcommunicationNumber2;
        private string strCommunicationNumberID3;
        private string strcommunicationNumber3;
        private const int intEDIHeader = 0;
        private const int intBatches = 1;
        private const int intClaimsinBatch = 2;
        private const int intSVCLines4Claim = 3;
        #endregion

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string Build837P(DataSet dsRawData)
        {
            try
            {
                string strWellFormed837P = "";
                ArrayList o837P = new ArrayList();
                HIPAAComponents.MNBilling oBills = new MNBilling();
                MNBilling.GE_FunctionalGroupTrailer oGE = new MNBilling.GE_FunctionalGroupTrailer();
                MNBilling.IEA_InterchangeControlTrailer oIEA = new MNBilling.IEA_InterchangeControlTrailer();
                int intSegmentCount;
                int intSVCLineCount;
                int intParent;
                int intHLLevel;
                string strBatchFilter;
                string strClaimsAndBatchFilter;

                //This section is used to set the header information required for all claims in all batches to be processed.
                intInterchangeID = int.Parse(dsRawData.Tables[intEDIHeader].Rows[0]["intInterchangeID"].ToString());
                strSenderIDQual = dsRawData.Tables[intEDIHeader].Rows[0]["strSenderIDQual"].ToString();
                strSenderID = dsRawData.Tables[intEDIHeader].Rows[0]["strSenderID"].ToString();
                strReceiverID = dsRawData.Tables[intEDIHeader].Rows[0]["strReceiverID"].ToString();
                strReceiverIDQual = dsRawData.Tables[intEDIHeader].Rows[0]["strReceiverIDQual"].ToString();
                blnAcknowledgement = bool.Parse(dsRawData.Tables[intEDIHeader].Rows[0]["blnAcknowledgement"].ToString());
                strRepetitionSeparator = dsRawData.Tables[intEDIHeader].Rows[0]["strRepetitionSeparator"].ToString();
                strElementSeparator = dsRawData.Tables[intEDIHeader].Rows[0]["strElementSeparator"].ToString();
                strUsage = dsRawData.Tables[intEDIHeader].Rows[0]["strUsage"].ToString();
                intSubmitterEDIContactID = int.Parse(dsRawData.Tables[intEDIHeader].Rows[0]["intSubmitterEDIContactID"].ToString());
                strContactFunctionCode = dsRawData.Tables[intEDIHeader].Rows[0]["strContactFunctionCode"].ToString();
                strContactName = dsRawData.Tables[intEDIHeader].Rows[0]["strContactName"].ToString();
                strCommunicationNumberID = dsRawData.Tables[intEDIHeader].Rows[0]["strCommunicationNumberID"].ToString();
                strCommunicationNumber = dsRawData.Tables[intEDIHeader].Rows[0]["strCommunicationNumber"].ToString();
                strCommunicationNumberID2 = dsRawData.Tables[intEDIHeader].Rows[0]["strCommunicationNumberID2"].ToString();
                strcommunicationNumber2 = dsRawData.Tables[intEDIHeader].Rows[0]["strcommunicationNumber2"].ToString();
                strCommunicationNumberID3 = dsRawData.Tables[intEDIHeader].Rows[0]["strCommunicationNumberID3"].ToString();
                strcommunicationNumber3 = dsRawData.Tables[intEDIHeader].Rows[0]["strCommunicationNumber3"].ToString();
                //************************************************************************************************************

                oBills.DefaultSep = "*";
                oBills.EndOfSegment = "~";
                oBills.AckRequested = blnAcknowledgement;
                
                //This section starts the building of an 837P. The process starts
                //with collecting all of the batches to be processes. This is normally
                //1 but it is possible to include more that 1 per file if the payer
                //will accept it that way.
                foreach (DataRow b in dsRawData.Tables[intBatches].Rows)
                {
                    intSegmentCount = 10;
                    intSVCLineCount = 1;
                    intParent = 1;
                    intHLLevel = 2;
                    o837P.Add(SetISA(b["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetGS(b["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetST(b["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBHT(b["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetSubmitterName(b, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetEDIContact(oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetReceiverName(b, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderLevel(1, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderName(b, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderAddress(b, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetCityStateZip(b, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetTaxID(b, oBills.DefaultSep, oBills.EndOfSegment));

                    strBatchFilter = "BatchNumber=" + b["BatchNumber"].ToString();
                //****************************************************************************************************************
                
                    //This section start iterating through each claim in a batch.
                    foreach(DataRow c in dsRawData.Tables[intClaimsinBatch].Select(strBatchFilter))
                    {
                        o837P.Add(SetSubscriberLevel(intHLLevel, intParent, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSBRSegment(b, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberName(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetBillingSubscriberAddress(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberCityStateZip(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberDemographics(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetPayerName(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetClaimSegment(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetPriorAuthorization(c, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetHISegment(c, oBills.DefaultSep, oBills.EndOfSegment));

                        strClaimsAndBatchFilter = "BatchNumber=" + b["BatchNumber"].ToString() + " And SubscriberID=" + c["ClaimsBatchID"].ToString();
                    //***************************************************************************************************************

                        foreach(DataRow s in dsRawData.Tables[intSVCLines4Claim].Select(strClaimsAndBatchFilter))
                            {
                                o837P.Add(SetLXSegment(intSVCLineCount, oBills.DefaultSep, oBills.EndOfSegment));
                                intSVCLineCount += 1;
                                o837P.Add(SetSV1Segment(s, oBills.DefaultSep, oBills.EndOfSegment));
                                o837P.Add(SetServiceDate(s, oBills.DefaultSep, oBills.EndOfSegment));
                                intSegmentCount = intSegmentCount + 3;
                            }
                        intSegmentCount = intSegmentCount + 10;
                        intHLLevel += 1;
                        intSVCLineCount = 1;
                    }
                    intSegmentCount = intSegmentCount + 1;
                    o837P.Add(SetSESegment(intSegmentCount, b["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetGESegment(oBills.DefaultSep, oBills.EndOfSegment, b["BatchNumber"].ToString()));
                    o837P.Add(SetIEASegment(oBills.DefaultSep, oBills.EndOfSegment, b["BatchNumber"].ToString()));
                }

                foreach(string strResult in o837P)
                {
                    strWellFormed837P = strWellFormed837P + strResult;
                }

                return strWellFormed837P;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region SpecialFunctions
        protected string SetIEASegment(string strDefaultSep, string strEOS, string strControlNo)
        {
            MNBilling.IEA_InterchangeControlTrailer IEA = new MNBilling.IEA_InterchangeControlTrailer();

            try
            {
                IEA.FunctionalGroupCount = 1;
                IEA.ControlNo = strControlNo;

                return "IEA" + strDefaultSep + "1" + strDefaultSep + strControlNo + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetGESegment(string strDefaultSep, string strEOS, string strControlNo)
        {
            MNBilling.GE_FunctionalGroupTrailer GE = new MNBilling.GE_FunctionalGroupTrailer();

            try
            {
                return "GE" + strDefaultSep + "1" + strDefaultSep + strControlNo + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSESegment(int intSVCLineCount, string strControlNo, string strDefaultSep, string strEOS)
        {
            SE_TransactionSetTrailer SE = new SE_TransactionSetTrailer();

            try
            {
                SE.SE01__TransactionSegmentCount = intSVCLineCount.ToString();
                SE.SE02__TransactionSetControlNumber = strControlNo;

                return "SE" + strDefaultSep + SE.SE01__TransactionSegmentCount + strDefaultSep + SE.SE02__TransactionSetControlNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetServiceDate(DataRow x, string strDefaultSep, string strEOS)
        {
            DTP_DateServiceDate_2400 DTP = new DTP_DateServiceDate_2400();

            try
            {
                DTP.DTP01__DateTimeQualifier = DTP_DateServiceDate_2400DTP01__DateTimeQualifier.Item472;
                DTP.DTP02__DateTimePeriodFormatQualifier = DTP_DateServiceDate_2400DTP02__DateTimePeriodFormatQualifier.D8;
                DTP.DTP03__ServiceDate = x["DateOfService"].ToString();

                return "DTP" + strDefaultSep + "472" + strDefaultSep + "D8" + strDefaultSep + DTP.DTP03__ServiceDate + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSV1Segment(DataRow x, string strDefaultSep, string strEOS)
        {
            SV1_ProfessionalService_2400 SV = new SV1_ProfessionalService_2400();

            try
            {
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_01_ProductOrServiceIDQualifier = SV1_ProfessionalService_2400SV101_CompositeMedicalProcedureIdentifier_2400SV101_01_ProductOrServiceIDQualifier.HC;
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_02_ProcedureCode = x["ProcedureCode"].ToString();
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_03_ProcedureModifier = x["Modifiers"].ToString();
                SV.SV102__LineItemChargeAmount = x["RatePerUnit"].ToString().Replace(".0000", ".00");
                SV.SV103__UnitOrBasisForMeasurementCode = SV1_ProfessionalService_2400SV103__UnitOrBasisForMeasurementCode.UN;
                SV.SV104__ServiceUnitCount = x["Units"].ToString().Replace(".0000", "");
                SV.SV105__PlaceOfServiceCode = x["POS"].ToString();
                SV.SV106 = "";
                //SV.SV107_CompositeDiagnosisCodePointer_2400.SV107_01_DiagnosisCodePointer = "1";
                SV.SV108 = "";
                //SV.SV109__EmergencyIndicator = SV1_ProfessionalService_2400SV109__EmergencyIndicator.Y;
                SV.SV110 = "";

                return "SV1" + strDefaultSep + "HC" + strElementSeparator + x["ProcedureCode"].ToString() + strElementSeparator +
                        x["Modifiers"].ToString() + strDefaultSep + SV.SV102__LineItemChargeAmount + strDefaultSep +
                        "UN" + strDefaultSep + SV.SV104__ServiceUnitCount + strDefaultSep + "14" + strDefaultSep + SV.SV106 + strDefaultSep + "1" +
                        strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetLXSegment(int intLineCount, string strDefaultSep, string strEOS)
        {
            LX_ServiceLineNumber_2400 LX = new LX_ServiceLineNumber_2400();

            try
            {
                LX.LX01__AssignedNumber = intLineCount.ToString();

                return "LX" + strDefaultSep + LX.LX01__AssignedNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetHISegment(DataRow x, string strDefaultSep, string strEOS)
        {
            HI_HealthCareDiagnosisCode_2300 HI = new HI_HealthCareDiagnosisCode_2300();


            try
            {
                //HI.HI01_HealthCareCodeInformation_2300.HI01_01_DiagnosisTypeCode = HI_HealthCareDiagnosisCode_2300HI01_HealthCareCodeInformation_2300HI01_01_DiagnosisTypeCode.ABK;
                //HI.HI01_HealthCareCodeInformation_2300.HI01_02_DiagnosisCode = x["DiagnosisCode"].ToString();

                return "HI" + strDefaultSep + "ABK" + strElementSeparator + x["DiagnosisCode"].ToString() + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetPriorAuthorization(DataRow x, string strDefaultSep, string strEOS)
        {
            REF_PriorAuthorization_2300 REF = new REF_PriorAuthorization_2300();

            try
            {
                return "REF" + strDefaultSep + "G1" + strDefaultSep + x["ServiceAgreementNo"].ToString() + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetClaimSegment(DataRow x, string strDefaultSep, string strEOS)
        {
            CLM_ClaimInformation_2300 CLM = new CLM_ClaimInformation_2300();
            CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300 CLM05 = new CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300();

            try
            {
                CLM.CLM01__PatientControlNumber = x["ClaimsBatchID"].ToString();
                CLM.CLM02__TotalClaimChargeAmount = string.Format("{0:0.00}", x["RatePerUnit"]);
                CLM.CLM03 = "";
                CLM.CLM04 = "";
                CLM05.CLM05_01_PlaceOfServiceCode = x["POS"].ToString();
                CLM05.CLM05_02_FacilityCodeQualifier = CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300CLM05_02_FacilityCodeQualifier.B;
                CLM05.CLM05_03_ClaimFrequencyCode = "1";
                CLM.CLM05_HealthCareServiceLocationInformation_2300 = CLM05;
                CLM.CLM06__ProviderOrSupplierSignatureIndicator = CLM_ClaimInformation_2300CLM06__ProviderOrSupplierSignatureIndicator.N;
                CLM.CLM07__AssignmentOrPlanParticipationCode = CLM_ClaimInformation_2300CLM07__AssignmentOrPlanParticipationCode.A;
                CLM.CLM08__BenefitsAssignmentCertificationIndicator = CLM_ClaimInformation_2300CLM08__BenefitsAssignmentCertificationIndicator.Y;
                CLM.CLM09__ReleaseOfInformationCode = CLM_ClaimInformation_2300CLM09__ReleaseOfInformationCode.Y;

                return "CLM" + strDefaultSep + CLM.CLM01__PatientControlNumber + strDefaultSep + CLM.CLM02__TotalClaimChargeAmount + strDefaultSep + CLM.CLM03 +
                        strDefaultSep + CLM.CLM04 + strDefaultSep + CLM05.CLM05_01_PlaceOfServiceCode + strElementSeparator + "B" + strElementSeparator + CLM05.CLM05_03_ClaimFrequencyCode +
                        strDefaultSep + "N" + strDefaultSep + "A" + strDefaultSep + "Y" + strDefaultSep + "Y" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetPayerName(DataRow x, string strDefaultSep, string strEOS)
        {
            NM1_PayerName_2010BB Q = new NM1_PayerName_2010BB();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_PayerName_2010BBNM101__EntityIdentifierCode.PR;
                Q.NM102__EntityTypeQualifier = NM1_PayerName_2010BBNM102__EntityTypeQualifier.Item2;
                Q.NM103__PayerName = x["Payer"].ToString();
                Q.NM104 = "";
                Q.NM105 = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_PayerName_2010BBNM108__IdentificationCodeQualifier.PI;
                Q.NM109__PayerIdentifier = x["PayerID"].ToString();

                return "NM1" + strDefaultSep + "PR" + strDefaultSep + "2" + strDefaultSep + Q.NM103__PayerName +
                        strDefaultSep + Q.NM104 + strDefaultSep + Q.NM105 + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "PI" + strDefaultSep + Q.NM109__PayerIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetEDIContact(string strDefaultSep, string strEOS)
        {
            PER_SubmitterEDIContactInformation_1000A P = new PER_SubmitterEDIContactInformation_1000A();
            string strReturn;

            try
            {
                P.PER01__ContactFunctionCode = PER_SubmitterEDIContactInformation_1000APER01__ContactFunctionCode.IC;
                P.PER02__SubmitterContactName = strContactName;
                switch (strCommunicationNumberID)
                {
                    case "EM":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.EM;
                        break;
                    case "FX":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }
                P.PER04__CommunicationNumber = strCommunicationNumber;
                switch (strCommunicationNumberID2)
                {
                    case "EM":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.EM;
                        break;
                    case "EX":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.EX;
                        break;
                    case "FX":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }
                P.PER06__CommunicationNumber = strcommunicationNumber2;
                switch (strCommunicationNumberID2)
                {
                    case "EM":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.EM;
                        break;
                    case "EX":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.EX;
                        break;
                    case "FX":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }
                P.PER08__CommunicationNumber = strcommunicationNumber3;

                strReturn = "PER" + strDefaultSep + "IC" + strDefaultSep + P.PER02__SubmitterContactName + strDefaultSep + P.PER03__CommunicationNumberQualifier + strDefaultSep + P.PER04__CommunicationNumber;
                if (strCommunicationNumberID2.Length == 2)
                {
                    strReturn = strReturn + strDefaultSep + P.PER05__CommunicationNumberQualifier + strDefaultSep + P.PER06__CommunicationNumber;
                    if (strCommunicationNumberID3.Length == 2)
                    {
                        strReturn = strReturn + strDefaultSep + P.PER07__CommunicationNumberQualifier + strDefaultSep + P.PER08__CommunicationNumber;
                    }
                }

                strReturn = strReturn + strEOS;

                return strReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberName(DataRow x, string strDefaultSep, string strEOS)
        {
            NM1_SubscriberName_2010BA Q = new NM1_SubscriberName_2010BA();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_SubscriberName_2010BANM101__EntityIdentifierCode.IL;
                Q.NM102__EntityTypeQualifier = NM1_SubscriberName_2010BANM102__EntityTypeQualifier.Item1;
                Q.NM103__SubscriberLastName = x["SubscriberLast"].ToString();
                Q.NM104__SubscriberFirstName = x["SubscriberFirst"].ToString();
                Q.NM105__SubscriberMiddleNameOrInitial = x["SubscriberMiddle"].ToString();
                Q.NM106 = "";
                Q.NM107__SubscriberNameSuffix = "";
                Q.NM108__IdentificationCodeQualifier = NM1_SubscriberName_2010BANM108__IdentificationCodeQualifier.MI;
                Q.NM109__SubscriberPrimaryIdentifier = x["SubscriberID"].ToString();

                return "NM1" + strDefaultSep + "IL" + strDefaultSep + "1" + strDefaultSep + Q.NM103__SubscriberLastName +
                        strDefaultSep + Q.NM104__SubscriberFirstName + strDefaultSep + Q.NM105__SubscriberMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107__SubscriberNameSuffix + strDefaultSep + "MI" + strDefaultSep + Q.NM109__SubscriberPrimaryIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSBRSegment(DataRow x, string strDefault, string strEOS)
        {
            SBR_SubscriberInformation_2000B SUB = new SBR_SubscriberInformation_2000B();

            try
            {
                SUB.SBR01__PayerResponsibilitySequenceNumberCode = SBR_SubscriberInformation_2000BSBR01__PayerResponsibilitySequenceNumberCode.U;
                SUB.SBR02__IndividualRelationshipCode = SBR_SubscriberInformation_2000BSBR02__IndividualRelationshipCode.Item18;
                SUB.SBR03__SubscriberGroupOrPolicyNumber = "";
                SUB.SBR04__SubscriberGroupName = "";
                SUB.SBR05__InsuranceTypeCode = SBR_SubscriberInformation_2000BSBR05__InsuranceTypeCode.Item12;
                SUB.SBR06 = "";
                SUB.SBR07 = "";
                SUB.SBR08 = "";
                SUB.SBR09__ClaimFilingIndicatorCode = SBR_SubscriberInformation_2000BSBR09__ClaimFilingIndicatorCode.MC;

                return "SBR" + strDefault + "U" + strDefault + "18" + strDefault + SUB.SBR03__SubscriberGroupOrPolicyNumber + strDefault +
                        SUB.SBR04__SubscriberGroupName + strDefault + "" + strDefault + SUB.SBR06 + strDefault + SUB.SBR07 + strDefault + SUB.SBR08 + strDefault +
                        "MC" + strEOS;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetTaxID(DataRow x, string strDefaultSep, string strEOS)
        {
            REF_BillingProviderTaxIdentification_2010AA TID = new REF_BillingProviderTaxIdentification_2010AA();

            try
            {
                TID.REF01__ReferenceIdentificationQualifier = REF_BillingProviderTaxIdentification_2010AAREF01__ReferenceIdentificationQualifier.EI;
                TID.REF02__BillingProviderTaxIdentificationNumber = x["StateID"].ToString();

                return "REF" + strDefaultSep + TID.REF01__ReferenceIdentificationQualifier + strDefaultSep + TID.REF02__BillingProviderTaxIdentificationNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberLevel(int intHID, int intParentID, string strDEfaultSep, string strEOS)
        {
            HL_SubscriberHierarchicalLevel_2000B HL = new HL_SubscriberHierarchicalLevel_2000B();

            try
            {
                HL.HL01__HierarchicalIDNumber = intHID.ToString();
                HL.HL02__HierarchicalParentIDNumber = intParentID.ToString();
                HL.HL03__HierarchicalLevelCode = HL_SubscriberHierarchicalLevel_2000BHL03__HierarchicalLevelCode.Item22;
                HL.HL04__HierarchicalChildCode = HL_SubscriberHierarchicalLevel_2000BHL04__HierarchicalChildCode.Item0;

                return "HL" + strDEfaultSep + HL.HL01__HierarchicalIDNumber + strDEfaultSep + HL.HL02__HierarchicalParentIDNumber +
                        strDEfaultSep + "22" + strDEfaultSep + "0" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetCityStateZip(DataRow x, string strDefault, string strEOS)
        {
            N4_BillingProviderCityStateZIPCode_2010AA BC = new N4_BillingProviderCityStateZIPCode_2010AA();

            try
            {
                BC.N401__BillingProviderCityName = "St. Paul";
                BC.N402__BillingProviderStateOrProvinceCode = "MN";
                BC.N403__BillingProviderPostalZoneOrZIPCode = "55107";

                return "N4" + strDefault + BC.N401__BillingProviderCityName + strDefault + BC.N402__BillingProviderStateOrProvinceCode + strDefault + BC.N403__BillingProviderPostalZoneOrZIPCode + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberDemographics(DataRow x, string strDefaultSep, string strEOS)
        {
            DMG_SubscriberDemographicInformation_2010BA d = new DMG_SubscriberDemographicInformation_2010BA();

            try
            {
                d.DMG01__DateTimePeriodFormatQualifier = DMG_SubscriberDemographicInformation_2010BADMG01__DateTimePeriodFormatQualifier.D8;
                d.DMG02__SubscriberBirthDate = x["SubscriberDOB"].ToString();
                switch (x["SubscriberGender"].ToString())
                {
                    case "M":
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.M;
                        break;
                    case "F":
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.F;
                        break;
                    default:
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.U;
                        break;
                }

                return "DMG" + strDefaultSep + "D8" + strDefaultSep + d.DMG02__SubscriberBirthDate + strDefaultSep + x["SubscriberGender"].ToString() + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberCityStateZip(DataRow x, string strDefault, string strEOS)
        {
            N4_SubscriberCityStateZIPCode_2010BA BC = new N4_SubscriberCityStateZIPCode_2010BA();

            try
            {
                BC.N401__SubscriberCityName = x["SubscriberCity"].ToString();
                BC.N402__SubscriberStateCode = x["SubscriberState"].ToString();
                BC.N403__SubscriberPostalZoneOrZIPCode = x["SubscriberZip"].ToString();

                return "N4" + strDefault + BC.N401__SubscriberCityName + strDefault + BC.N402__SubscriberStateCode + strDefault + BC.N403__SubscriberPostalZoneOrZIPCode + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingSubscriberAddress(DataRow x, string strDefaultSep, string strEOS)
        {
            N3_SubscriberAddress_2010BA A = new N3_SubscriberAddress_2010BA();
            string strResults;

            try
            {
                A.N301__SubscriberAddressLine = x["SubscriberAddress1"].ToString();
                A.N302__SubscriberAddressLine = x["SubscriberAddress2"].ToString();

                strResults = "N3" + strDefaultSep + A.N301__SubscriberAddressLine;
                if (A.N302__SubscriberAddressLine.Length > 0)
                {
                    strResults = strResults + strDefaultSep + A.N302__SubscriberAddressLine;
                }
                strResults = strResults + strEOS;

                return strResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderAddress(DataRow x, string strDefaultSep, string strEOS)
        {
            N3_BillingProviderAddress_2010AA A = new N3_BillingProviderAddress_2010AA();

            try
            {
                A.N301__BillingProviderAddressLine = "30 East Plato Boulevard";

                return "N3" + strDefaultSep + A.N301__BillingProviderAddressLine + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderName(DataRow x, string strDefaultSep, string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_BillingProviderName_2010AA Q = new NM1_BillingProviderName_2010AA();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_BillingProviderName_2010AANM101__EntityIdentifierCode.Item85;
                Q.NM102__EntityTypeQualifier = NM1_BillingProviderName_2010AANM102__EntityTypeQualifier.Item2;
                Q.NM103__BillingProviderLastOrOrganizationalName = x["Biller"].ToString();
                Q.NM104__BillingProviderFirstName = "";
                Q.NM105__BillingProviderMiddleNameOrInitial = "";
                Q.NM106 = "";
                Q.NM107__BillingProviderNameSuffix = "";
                Q.NM108__IdentificationCodeQualifier = NM1_BillingProviderName_2010AANM108__IdentificationCodeQualifier.XX;
                Q.NM109__BillingProviderIdentifier = x["NPI"].ToString();

                return "NM1" + strDefaultSep + "85" + strDefaultSep + "2" + strDefaultSep + Q.NM103__BillingProviderLastOrOrganizationalName +
                        strDefaultSep + Q.NM104__BillingProviderFirstName + strDefaultSep + Q.NM105__BillingProviderMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107__BillingProviderNameSuffix + strDefaultSep + "XX" + strDefaultSep + Q.NM109__BillingProviderIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderLevel(int intHID, string strDefaultSep, string strEOS)
        {
            MNBilling oHL = new MNBilling();
            HL_BillingProviderHierarchicalLevel_2000A Y = new HL_BillingProviderHierarchicalLevel_2000A();

            try
            {
                Y.HL01__HierarchicalIDNumber = "1";
                Y.HL02 = "";
                Y.HL03__HierarchicalLevelCode = HL_BillingProviderHierarchicalLevel_2000AHL03__HierarchicalLevelCode.Item20;
                Y.HL04__HierarchicalChildCode = HL_BillingProviderHierarchicalLevel_2000AHL04__HierarchicalChildCode.Item1;

                return "HL" + strDefaultSep + Y.HL01__HierarchicalIDNumber + strDefaultSep + Y.HL02 + strDefaultSep + "20" + strDefaultSep + "1" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetReceiverName(DataRow x, string strDefaultSep, string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_ReceiverName_1000B Q = new NM1_ReceiverName_1000B();

            try
            {

                Q.NM101__EntityIdentifierCode = NM1_ReceiverName_1000BNM101__EntityIdentifierCode.Item40;
                Q.NM102__EntityTypeQualifier = NM1_ReceiverName_1000BNM102__EntityTypeQualifier.Item2;
                Q.NM103__ReceiverName = x["Payer"].ToString();
                Q.NM104 = "";
                Q.NM105 = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_ReceiverName_1000BNM108__IdentificationCodeQualifier.Item46;
                Q.NM109__ReceiverPrimaryIdentifier = x["PayerID"].ToString();

                return "NM1" + strDefaultSep + "40" + strDefaultSep + "2" + strDefaultSep + Q.NM103__ReceiverName +
                        strDefaultSep + Q.NM104 + strDefaultSep + Q.NM105 + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "46" + strDefaultSep + Q.NM109__ReceiverPrimaryIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubmitterName(DataRow x, string strDefaultSep, string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_SubmitterName_1000A Q = new NM1_SubmitterName_1000A();

            try
            {

                Q.NM101__EntityIdentifierCode = NM1_SubmitterName_1000ANM101__EntityIdentifierCode.Item41;
                Q.NM102__EntityTypeQualifier = NM1_SubmitterName_1000ANM102__EntityTypeQualifier.Item2;
                Q.NM103__SubmitterLastOrOrganizationName = x["Biller"].ToString();
                Q.NM104__SubmitterFirstName = "";
                Q.NM105__SubmitterMiddleNameOrInitial = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_SubmitterName_1000ANM108__IdentificationCodeQualifier.Item46;
                Q.NM109__SubmitterIdentifier = x["NPI"].ToString();

                return "NM1" + strDefaultSep + "41" + strDefaultSep + "2" + strDefaultSep + Q.NM103__SubmitterLastOrOrganizationName +
                        strDefaultSep + Q.NM104__SubmitterFirstName + strDefaultSep + Q.NM105__SubmitterMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "46" + strDefaultSep + Q.NM109__SubmitterIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBHT(string strControlNo, string strDefaultSep, string strEOS)
        {
            MNBilling oBHT = new MNBilling();
            BHT_BeginningOfHierarchicalTransaction P = new BHT_BeginningOfHierarchicalTransaction();

            try
            {

                P.BHT01__HierarchicalStructureCode = BHT_BeginningOfHierarchicalTransactionBHT01__HierarchicalStructureCode.Item0019;
                P.BHT02__TransactionSetPurposeCode = BHT_BeginningOfHierarchicalTransactionBHT02__TransactionSetPurposeCode.Item00;
                P.BHT03__OriginatorApplicationTransactionIdentifier = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                P.BHT04__TransactionSetCreationDate = System.DateTime.Now.ToString("yyyyMMdd");
                P.BHT05__TransactionSetCreationTime = System.DateTime.Now.ToString("HHmm");
                P.BHT06__ClaimOrEncounterIdentifier = BHT_BeginningOfHierarchicalTransactionBHT06__ClaimOrEncounterIdentifier.CH;

                return "BHT" + strDefaultSep + "0019" + strDefaultSep + "00" + strDefaultSep + P.BHT03__OriginatorApplicationTransactionIdentifier +
                    strDefaultSep + P.BHT04__TransactionSetCreationDate + strDefaultSep + P.BHT05__TransactionSetCreationTime + strDefaultSep + "CH" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetST(string strControlNo, string strDefaultSep, string strEOS)
        {
            MNBilling oST = new MNBilling();

            try
            {
                oST.ST_TransactionSetHeader = new ST_TransactionSetHeader();
                oST.ST_TransactionSetHeader.ST02__TransactionSetControlNumber = strControlNo;
                oST.ST_TransactionSetHeader.ST01__TransactionSetIdentifierCode = ST_TransactionSetHeaderST01__TransactionSetIdentifierCode.Item837;
                oST.ST_TransactionSetHeader.ST03__ImplementationGuideVersionName = "005010X222A1";

                return "ST" + strDefaultSep + "837" + strDefaultSep + oST.ST_TransactionSetHeader.ST02__TransactionSetControlNumber + strDefaultSep + oST.ST_TransactionSetHeader.ST03__ImplementationGuideVersionName + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetGS(string strControlNo, string strDefaultSep, string strEOS)
        {
            MNBilling.GS_FunctionalGroupHeader oGS = new MNBilling.GS_FunctionalGroupHeader();

            try
            {
                oGS.ControlNo = strControlNo;
                oGS.SetDefaultSep = strDefaultSep;
                oGS.SetEOS = strEOS;
                oGS.GSID = oGS.GetIDCodeFromString("HC");
                oGS.ReceiverID = strReceiverID;
                oGS.SenderID = strSenderID;

                return oGS.GS_Segment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Setting Up the ISA Segment
        /// </summary>
        protected string SetISA(string strControlNo, string strDefaultSep, string strEOS)
        {
            MNBilling.ISA_InterchangeControlHeader oISA = new MNBilling.ISA_InterchangeControlHeader();
            try
            {
                oISA.ControlNo = strControlNo;
                oISA.Acknowledgement(blnAcknowledgement);
                oISA.ReceiverID = strReceiverID;
                oISA.ReceiverQual = oISA.ReturnXQualValueFromString(strReceiverIDQual);
                oISA.SenderQual = oISA.ReturnXQualValueFromString(strSenderIDQual);
                oISA.SenderID = strSenderID;
                oISA.EleSep = strElementSeparator;
                oISA.RepSep = strRepetitionSeparator;
                oISA.Usage = strUsage;
                oISA.SetDefaultSep = strDefaultSep;
                oISA.SetEOS = strEOS;

                return oISA.ISASegment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
