ROOT := $(dir $(lastword $(MAKEFILE_LIST)))../..

LIB_DIR = ../../lib

COMPONENT = SEN-SCD30

CS_LIB_SOURCES = \
	$(LIB_DIR)/surface/Actuator.cs \
	$(LIB_DIR)/surface/EventTimer.cs \
	$(LIB_DIR)/surface/HALComponent.cs \
	$(LIB_DIR)/surface/HALId.cs \
	$(LIB_DIR)/surface/HALObserver.cs \
	$(LIB_DIR)/surface/Property.cs \
	$(LIB_DIR)/surface/Sensor.cs \
	$(LIB_DIR)/surface/TimerObserver.cs \
	$(LIB_DIR)/surface/openEL.cs \
	$(LIB_DIR)/surface/openEL_registryConfig.cs \
	$(LIB_DIR)/device/$(COMPONENT)/openEL_SensorSCD30.cs \

TARGET = openel_test.exe

TARGET_SOURCES = openel_test.cs

.PHONY : all
all : 
	csc -unsafe -define:DEV -main:OpenELTest -out:$(TARGET) \
	$(CS_LIB_SOURCES) \
	$(TARGET_SOURCES)
	chmod +x $(TARGET) 

.PHONY: clean
clean:
	rm $(TARGET)

