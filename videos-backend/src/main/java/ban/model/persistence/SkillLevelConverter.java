package ban.model.persistence;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMarshaller;

/**
 * Created by bnorrish on 10/19/15.
 */

// Converts the complex type DimensionType to a string and vice-versa.
public class SkillLevelConverter implements DynamoDBMarshaller<SkillLevel> {

  @Override
  public String marshall(SkillLevel value) {
    return value.toString();
  }

  @Override
  public SkillLevel unmarshall(Class<SkillLevel> dimensionType, String value) {
    return SkillLevel.valueOf(value);
  }
}

