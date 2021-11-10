IF EXISTS ( SELECT  1
            FROM    dbo.net_LogoTransferSettings
            WHERE   EntegrationName = @EntegrationName)
BEGIN

  UPDATE    dbo.net_LogoTransferSettings
  SET       ClientTransferPrefixCode = @ClientTransferPrefixCode
           ,ClientTransferSpeCode1 = @ClientTransferSpeCode1
           ,ClientTransferSpeCode2 = @ClientTransferSpeCode2
           ,ClientTransferSpeCode3 = @ClientTransferSpeCode3
           ,ClientTransferSpeCode4 = @ClientTransferSpeCode4
           ,ClientTransferSpeCode5 = @ClientTransferSpeCode5
           ,ClientTransferAccountCode = @ClientTransferAccountCode
           ,ClientTransferPaymentCode = @ClientTransferPaymentCode
           ,ClientTransferTradingGroup = @ClientTransferTradingGroup
           ,ClientTransferProjectCode = @ClientTransferProjectCode
           ,ClientTransferAuthCode = @ClientTransferAuthCode

		   ,TransferFicheProjectCode = @TransferFicheProjectCode

		   ,OrderTransferWareHouseNr = @OrderTransferWareHouseNr
		   ,OrderTransferDivisionNr = @OrderTransferDivisionNr
		   ,OrderTransferUnitPriceRoundingNumberOfDigits = @OrderTransferUnitPriceRoundingNumberOfDigits

		   ,OrderTransferStatus = @OrderTransferStatus

		   ,OrderTransferRetransferTransferedOrder = @OrderTransferRetransferTransferedOrder
		   ,OrderTransferAddRowsIfOrder = @OrderTransferAddRowsIfOrder
		   ,OrderTransferGroupByOrderNumber = @OrderTransferGroupByOrderNumber
		   ,OrderTransferTransferToShippingAddress = @OrderTransferTransferToShippingAddress

		   ,OrderTransferTradingGroup = @OrderTransferTradingGroup
		   ,OrderTransferProjectCode = @OrderTransferProjectCode
		   ,OrderTransferDiscountCouponCode = @OrderTransferDiscountCouponCode
		   ,OrderTransferServiceChargeCode = @OrderTransferServiceChargeCode
		   ,OrderTransferLateChargeCode = @OrderTransferLateChargeCode
		   ,OrderTransferSalesManCode = @OrderTransferSalesManCode
		   ,OrderTransferDoCode = @OrderTransferDoCode
           ,OrderTransferSpeCode = @OrderTransferSpeCode
           ,OrderTransferPaymentCode = @OrderTransferPaymentCode
           ,OrderTransferArpShippmentCode = @OrderTransferArpShippmentCode
           ,OrderTransferArpCode = @OrderTransferArpCode                              
           ,OrderTransferAuthCode = @OrderTransferAuthCode                              
           ,EntegrationName = @EntegrationName                    
           
  WHERE     EntegrationName = @EntegrationName

END
ELSE
BEGIN

  INSERT INTO   dbo.net_LogoTransferSettings
  (
    ClientTransferPrefixCode
   ,ClientTransferSpeCode1
   ,ClientTransferSpeCode2
   ,ClientTransferSpeCode3
   ,ClientTransferSpeCode4
   ,ClientTransferSpeCode5
   ,ClientTransferAccountCode
   ,ClientTransferPaymentCode
   ,ClientTransferTradingGroup
   ,ClientTransferProjectCode

   ,TransferFicheProjectCode

   ,OrderTransferWareHouseNr
   ,OrderTransferDivisionNr
   ,OrderTransferUnitPriceRoundingNumberOfDigits

   ,OrderTransferStatus

   ,OrderTransferRetransferTransferedOrder
   ,OrderTransferAddRowsIfOrder
   ,OrderTransferGroupByOrderNumber

   ,OrderTransferTradingGroup
   ,OrderTransferProjectCode
   ,OrderTransferDiscountCouponCode
   ,OrderTransferServiceChargeCode
   ,OrderTransferLateChargeCode
   ,OrderTransferSalesManCode
   ,OrderTransferDoCode
   ,OrderTransferSpeCode
   ,OrderTransferPaymentCode
   ,OrderTransferArpShippmentCode
   ,OrderTransferArpCode
   
   ,EntegrationName
  )
  VALUES
  ( @ClientTransferPrefixCode
   ,@ClientTransferSpeCode1
   ,@ClientTransferSpeCode2
   ,@ClientTransferSpeCode3
   ,@ClientTransferSpeCode4
   ,@ClientTransferSpeCode5
   ,@ClientTransferAccountCode
   ,@ClientTransferPaymentCode
   ,@ClientTransferTradingGroup
   ,@ClientTransferProjectCode

   ,@TransferFicheProjectCode

   ,@OrderTransferWareHouseNr
   ,@OrderTransferDivisionNr
   ,@OrderTransferUnitPriceRoundingNumberOfDigits

   ,@OrderTransferStatus

   ,@OrderTransferRetransferTransferedOrder
   ,@OrderTransferAddRowsIfOrder
   ,@OrderTransferGroupByOrderNumber

   ,@OrderTransferTradingGroup
   ,@OrderTransferProjectCode
   ,@OrderTransferDiscountCouponCode
   ,@OrderTransferServiceChargeCode
   ,@OrderTransferLateChargeCode
   ,@OrderTransferSalesManCode
   ,@OrderTransferDoCode
   ,@OrderTransferSpeCode
   ,@OrderTransferPaymentCode
   ,@OrderTransferArpShippmentCode
   ,@OrderTransferArpCode

   ,@EntegrationName
   )

END