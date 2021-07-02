ROOT := $(dir $(lastword $(MAKEFILE_LIST)))../..

LIB_DIR = ../../lib

COMPONENT_1 = SEN-SCD30
COMPONENT_2 = DIA-Diarkis

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
	$(LIB_DIR)/device/$(COMPONENT_1)/openEL_SensorSCD30.cs \
	$(LIB_DIR)/device/$(COMPONENT_2)/openEL_SensorDiarkis.cs \

TARGET = demo.exe

TARGET_SOURCES = demo.cs diarkis_test.cs

.PHONY : all
all : 
	csc -unsafe -define:DEV -out:$(TARGET) \
    -reference:$(LIB_DIR)/device/$(COMPONENT_2)/Diarkis.dll \
	$(CS_LIB_SOURCES) \
	$(TARGET_SOURCES)
	chmod +x $(TARGET) 

.PHONY: clean
clean:
	rm $(TARGET)

